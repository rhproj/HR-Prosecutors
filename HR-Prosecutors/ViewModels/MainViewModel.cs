using EosKadriLibrary;
using HR_Prosecutors.Commands;
using HR_Prosecutors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HR_Prosecutors.ViewModels
{
    class MainViewModel : ViewModel
    {
        private int _title = 7;
        public int Title
        {
            get { return _title; }
            set { _title = value; }
        }

        K092Context dbContext = new K092Context();

        private int _tabIndex;
        public int TabIndex 
        {
            get { return _tabIndex; }
            set { _tabIndex = value; OnPropertyChanged(); } 
        }


        private IList<PersonActive> _onActivePositionsList;
        public IList<PersonActive> OnActivePositionsList 
        {
            get { return _onActivePositionsList; }
            set { _onActivePositionsList = value; OnPropertyChanged(); } // 
        }

        private IList<PersonActive> _onActiveProsecutorsList;
        public IList<PersonActive> OnActiveProsecutorsList 
        {
            get { return _onActiveProsecutorsList; }
            set { _onActiveProsecutorsList = value; OnPropertyChanged(); } 
        }

        private IList<PersonActive> _onActiveSpecialistsList;
        public IList<PersonActive> OnActiveSpecialistsList 
        {
            get { return _onActiveSpecialistsList; }
            set { _onActiveSpecialistsList = value; OnPropertyChanged(); }
        }

        public IList<PersonSL> PersonSList { get; }

        /// <summary> Работники прокуратуры, незадействованые в настоящее время</summary>
        public IList<PersonSL> PersonDList { get; set; }

        private string _nameToSearch;
        public string NameToSearch
        {
            get { return _nameToSearch; }
            set { _nameToSearch = value; OnPropertyChanged(); }
        }


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


        #endregion

        public MainViewModel()
        {
            //var dbContext = new K092Context();

            #region Dummy data
            //FioList = new List<CADRE_VIEW_PERSONSL>
            //{
            //    new CADRE_VIEW_PERSONSL
            //    {
            //        ISN_PERSON = "12",
            //        FIO = "Иванов",
            //        DOL = "Отд 13",
            //        POD = "Сарман",
            //        ORG = "Прок"
            //    }
            //}; 
            #endregion

            OnActivePositionsList = LoadActive();

            OnActiveProsecutorsList = LoadActiveProsecutors();
            
            OnActiveSpecialistsList = LoadActiveSpecialists();

            PersonSList = LoadRaw();

            PersonDList = LoadNotActive();

            SearchCommand = new RelayCommand(OnSearchCommandExecuted, CanSearchCommandExecute);



            #region MyRegion
            //FioList = (from pSL in dbContext.CADRE_VIEW_PERSONSL
            //           select new
            //           {
            //               ISN_PERSON = pSL.ISN_PERSON,
            //               FIO = pSL.FIO,
            //               DOL = pSL.DOL,
            //               POD = pSL.POD,
            //               ORG = pSL.ORG
            //           }).ToList<CADRE_VIEW_PERSONSL>();


            //var activeStaff = from pSL in dbContext.CADRE_VIEW_PERSONSL
            //                   join fio in dbContext.CADRE_VIEW_FIO on
            //                   pSL.ISN_PERSON equals fio.ISN_PERSON
            //                   orderby fio.FIO
            //                   select new
            //                   {
            //                       FIO = fio.FIO,
            //                       DOL = pSL.DOL,
            //                       POD = pSL.POD,
            //                       ISN_PERSON = pSL.ISN_PERSON
            //                   }; 
            #endregion
        }

        private IEnumerable<PersonActive> SearchPerson(IEnumerable<PersonActive> listToSearch, string name)
        {
            var result = listToSearch.Where(p => p.FIO.Contains(name, StringComparison.InvariantCultureIgnoreCase));

            return result; //new ObservableCollection<PersonActive>(result);
        }

        #region LOAD queries
        private List<PersonActive> LoadActiveSpecialists()
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
            return  (from fio in dbContext.CADRE_VIEW_FIO
                    join pSL in dbContext.CADRE_VIEW_PERSONSL
                    on fio.ISN_PERSON equals pSL.ISN_PERSON
                    orderby fio.FIO
                    select new PersonActive
                    {
                        FIO = fio.FIO,
                        Position = pSL.DOL,
                        Department = pSL.POD
                    }).ToList();
  //new ObservableCollection<PersonActive>(list);
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

            return result.Select(p => p.FirstOrDefault()).OrderBy(p=>p.FIO).ToList();
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
    }
}


//{
//    FIO = pSL.FIO,
//    DOL = pSL.DOL,
//    POD = pSL.POD
//};