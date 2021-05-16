using EosKadriLibrary;
using HR_Prosecutors.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Prosecutors.ViewModels
{
    class MainViewModel
    {
        private int _title = 7;
        public int Title
        {
            get { return _title = 7; }
            set { _title = value; }
        }

        public IList<PersonSL> PersonSList { get; }

        public IList<PersonActive> OnActivePositionsList { get; }

        public IList<PersonActive> OnActiveProsecutorsList { get; }

        public IList<PersonActive> OnActiveSpecialistsList { get; }

        public MainViewModel()
        {
            var dbContext = new K092Context();

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

            OnActivePositionsList = LoadActive(dbContext);

            PersonSList = LoadRaw(dbContext);

            OnActiveProsecutorsList = LoadActiveProsecutors(dbContext);

            OnActiveSpecialistsList = LoadActiveSpecialists(dbContext);

            //OnActivePositionsList = (from fio in dbContext.CADRE_VIEW_FIO
            //                         join pSL in dbContext.CADRE_VIEW_PERSONSL
            //                         on fio.ISN_PERSON equals pSL.ISN_PERSON
            //                         orderby fio.FIO
            //                         select new PersonActive()
            //                         {
            //                             FIO = fio.FIO,
            //                             Position = pSL.DOL,
            //                             Department = pSL.POD
            //                         }).ToList();


            //PersonSList = (from pSL in dbContext.CADRE_VIEW_PERSONSL
            //               select new PersonSL
            //               {
            //                   Isn = num, //pSL.ISN_PERSON,
            //                   FIO = pSL.FIO,
            //                   Position = pSL.DOL,
            //                   Department = pSL.POD,
            //                   Organization = pSL.ORG
            //               }).ToList();

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

        private List<PersonActive> LoadActiveSpecialists(K092Context dbContext)
        {
            var list = (from fio in dbContext.CADRE_VIEW_FIO
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

            return list;
        }

        private List<PersonActive> LoadActiveProsecutors(K092Context dbContext)
        {
            var list = (from fio in dbContext.CADRE_VIEW_FIO
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

            return list;
        }

        private List<PersonActive> LoadActive(K092Context dbContext)
        {
            var list = (from fio in dbContext.CADRE_VIEW_FIO
                        join pSL in dbContext.CADRE_VIEW_PERSONSL
                        on fio.ISN_PERSON equals pSL.ISN_PERSON
                        orderby fio.FIO
                        select new PersonActive
                        {
                            FIO = fio.FIO,
                            Position = pSL.DOL,
                            Department = pSL.POD
                        }).ToList();

            return list;
        }

        private List<PersonSL> LoadRaw(K092Context dbContext)
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
    }
}


//{
//    FIO = pSL.FIO,
//    DOL = pSL.DOL,
//    POD = pSL.POD
//};