using EosKadriLibrary;
using HR_Prosecutors.Commands;
using HR_Prosecutors.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HR_Prosecutors.ViewModels
{
    class MainViewModel : ViewModel
    {
        K092Context dbContext = new K092Context();

        #region PROPERTIES
        private int _tabIndex;
        public int TabIndex
        {
            get { return _tabIndex; }
            set { _tabIndex = value; OnPropertyChanged(); }
        }

        private string _nameToSearch;
        /// <summary> Name to search</summary>
        public string NameToSearch
        {
            get { return _nameToSearch; }
            set { _nameToSearch = value; OnPropertyChanged(); }
        }


        /// <summary> HR-DB name to connect to </summary>
        public string DbName { get { return $"База данных: {dbContext.Database.Connection.Database}"; } }


        private IEnumerable<PersonActive> _onActivePositionsList;
        /// <summary> Staff on positions </summary>
        public IEnumerable<PersonActive> OnActivePositionsList
        {
            get { return _onActivePositionsList; }
            set { _onActivePositionsList = value; OnPropertyChanged(); }
        }

        private IEnumerable<PersonActive> _onActiveProsecutorsList;
        /// <summary> Prosecutors on positions </summary>
        public IEnumerable<PersonActive> OnActiveProsecutorsList
        {
            get { return _onActiveProsecutorsList; }
            set { _onActiveProsecutorsList = value; OnPropertyChanged(); }
        }

        private IEnumerable<PersonActive> _onActiveSpecialistsList;
        /// <summary> Specialists on positions </summary>
        public IEnumerable<PersonActive> OnActiveSpecialistsList
        {
            get { return _onActiveSpecialistsList; }
            set { _onActiveSpecialistsList = value; OnPropertyChanged(); }
        }

        /// <summary> Active staff members </summary>
        public IEnumerable<PersonSL> PersonSList { get; }

        /// <summary> Inactive staff members </summary>
        public IEnumerable<PersonSL> PersonDList { get; set; }

        #endregion

        #region COMMANDS
        public ICommand SearchCommand { get; }
        private bool CanSearchCommandExecute(object p)  //w/t object ругается CTOR
        {
            if (TabIndex < 3)  //OnActivePositionsList == null || 
                return true;

            return false;
        }
        private void OnSearchCommandExecuted(object p)
        {
            if (TabIndex == 0)
            {
                OnActivePositionsList = SearchPerson(LoadActive(), NameToSearch).ToList();
            }
            else if (TabIndex == 1)
            {
                OnActiveProsecutorsList = SearchPerson(LoadActiveProsecutors(), NameToSearch).ToList();
            }
            else if (TabIndex == 2)
            {
                OnActiveSpecialistsList = SearchPerson(LoadActiveSpecialists(), NameToSearch).ToList();
            }
        }


        public ICommand SaveExcelCommand { get; }
        private bool CanSaveExcelCommandExecute(object p)
        {
            if (OnActivePositionsList != null)
                return true;
            return false;
        }
        private void OnSaveExcelCommandExecuted(object p)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            SaveFileDialog sfD = new SaveFileDialog();
            sfD.DefaultExt = ".xlsx";
            sfD.Filter = "Файл Excel (.xlsx)|*.xlsx";

            if (TabIndex == 0)
            {
                sfD.FileName = $"Кадры (на {DateTime.Now.ToString("dd.MM.yy")})";
                if (sfD.ShowDialog() == true)
                {
                    var path = new FileInfo(sfD.FileName);

                    SaveExcel(OnActivePositionsList, path, "Кадры");
                }
            }
            else if (TabIndex == 1)
            {
                sfD.FileName = $"Оперативные сотрудники (на {DateTime.Now.ToString("dd.MM.yy")})";
                if (sfD.ShowDialog() == true)
                {
                    var path = new FileInfo(sfD.FileName);

                    SaveExcel(OnActiveProsecutorsList, path, "Прокуроры");
                }
            }
            else if (TabIndex == 2)
            {
                sfD.FileName = $"Специалисты (на {DateTime.Now.ToString("dd.MM.yy")})";
                if (sfD.ShowDialog() == true)
                {
                    var path = new FileInfo(sfD.FileName);

                    SaveExcel(OnActiveSpecialistsList, path, "Специалисты");
                }
            }
            else if (TabIndex == 3)
            {
                sfD.FileName = $"ISN (на {DateTime.Now.ToString("dd.MM.yy")})";
                if (sfD.ShowDialog() == true)
                {
                    var path = new FileInfo(sfD.FileName);

                    SaveExcel(PersonSList, path, "ISN");
                }
            }
            else
            {
                sfD.FileName = $"Не на должностях (на {DateTime.Now.ToString("dd.MM.yy")})";
                if (sfD.ShowDialog() == true)
                {
                    var path = new FileInfo(sfD.FileName);

                    SaveExcel(PersonDList, path, "Не на должностях");
                }
            }
        }

        #endregion

        public MainViewModel()
        {
            OnActivePositionsList = LoadActive();

            OnActiveProsecutorsList = LoadActiveProsecutors();
            
            OnActiveSpecialistsList = LoadActiveSpecialists();

            PersonSList = LoadRaw();

            PersonDList = LoadNotActive();

            SearchCommand = new RelayCommand(OnSearchCommandExecuted, CanSearchCommandExecute);

            SaveExcelCommand = new RelayCommand(OnSaveExcelCommandExecuted, CanSaveExcelCommandExecute);
        }

        #region METHODS
        private IEnumerable<PersonActive> SearchPerson(IEnumerable<PersonActive> listToSearch, string name)
        {
            var result = listToSearch.Where(p => p.FIO.Contains(name, StringComparison.InvariantCultureIgnoreCase));

            return result; //new ObservableCollection<PersonActive>(result);
        }

        private void SaveExcel(IEnumerable<IPerson> StaffList, FileInfo file, string header)
        {
            if (file.Exists)
                file.Delete();

            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(header);

                #region Header
                ws.Cells[Address: "A1"].Value = $"{header} на {DateTime.Now.ToString("dd.MM.yyyy")}";
                ws.Cells[Address: "A1:C1"].Merge = true;
                ws.Row(row: 1).Style.Font.Size = 18;
                ws.Row(row: 1).Style.Font.Color.SetColor(Color.DarkGray);
                ws.Row(row: 2).Style.Font.Bold = true;
                #endregion

                ExcelRangeBase range = ws.Cells[Address: "A2"].LoadFromCollection(StaffList, PrintHeaders: true); //LFC allows to pass INmbl<T> (in our case List<PM>),  PrintHeaders - will take prop names for headers  //"A2"-starting point
                range.AutoFitColumns(); //so we see everything
                ws.Column(col: 3).Width = 30;

                package.SaveAsync();

                //w/o using make sure to close excel: package.Dispose();
            }
        }

        #region LOAD queries
        private IList<PersonActive> LoadActiveSpecialists()
        {
            return (from fio in dbContext.CADRE_VIEW_FIO
                    join pSL in dbContext.CADRE_VIEW_PERSONSL
                    on fio.ISN_PERSON equals pSL.ISN_PERSON
                    where !(pSL.DOL.Contains("прок") || pSL.DOL.Contains("началь"))
                    orderby fio.FIO
                    select new PersonActive
                    {
                        FIO = fio.FIO,
                        Position = pSL.DOL,
                        Department = pSL.POD
                    }).ToList();
        }

        private IList<PersonActive> LoadActiveProsecutors()
        {
            return (from fio in dbContext.CADRE_VIEW_FIO
                    join pSL in dbContext.CADRE_VIEW_PERSONSL
                    on fio.ISN_PERSON equals pSL.ISN_PERSON
                    where pSL.DOL.Contains("прок") || pSL.DOL.Contains("началь")
                    orderby fio.FIO
                    select new PersonActive
                    {
                        FIO = fio.FIO,
                        Position = pSL.DOL,
                        Department = pSL.POD
                    }).ToList();
        }

        private IList<PersonActive> LoadActive()
        {
            return (from fio in dbContext.CADRE_VIEW_FIO
                    join pSL in dbContext.CADRE_VIEW_PERSONSL
                    on fio.ISN_PERSON equals pSL.ISN_PERSON
                    orderby fio.FIO
                    select new PersonActive
                    {
                        FIO = fio.FIO,
                        Position = pSL.DOL,
                        Department = pSL.POD
                    }).ToList();
        }

        private IList<PersonSL> LoadNotActive()
        {
            var result = (from pD in dbContext.CADRE_VIEW_PERSONSL2
                          join pA in dbContext.CADRE_VIEW_PERSONSL
                          on pD.ISN_PERSON equals pA.ISN_PERSON
                          into InterstagePSL
                          from pS in InterstagePSL.DefaultIfEmpty()
                          where (pS.ISN_PERSON == null)
                          select new PersonSL
                          {
                              Isn = pD.ISN_PERSON,
                              FIO = pD.FIO,
                              Position = pD.DOL,
                              Department = pD.POD,
                              Organization = pD.ORG
                          }).GroupBy(p => p.Isn);

            return result.Select(p => p.FirstOrDefault()).OrderBy(p => p.FIO).ToList();
        }

        private IList<PersonSL> LoadRaw()
        {
            var list = dbContext.CADRE_VIEW_PERSONSL.Select(pSL => new PersonSL
            {
                Isn = pSL.ISN_PERSON,
                FIO = pSL.FIO,
                Position = pSL.DOL,
                Department = pSL.POD,
                Organization = pSL.ORG
            }).ToList();

            return list;
        }
        #endregion 
        #endregion
    }
}