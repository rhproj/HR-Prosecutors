using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRtest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new K092DBContext();

            var activeStaff = from pSL in dbContext.CADRE_VIEW_PERSONSL
                               join fio in dbContext.CADRE_VIEW_FIO on
                               pSL.ISN_PERSON equals fio.ISN_PERSON
                               orderby fio.FIO
                               select new
                               {
                                   FIO = fio.FIO,
                                   DOL = pSL.DOL,
                                   POD = pSL.POD,
                                   ISN_PERSON = pSL.ISN_PERSON
                               };  //).ToList(); - not needed cuz it's IQuariable already wich means foreach works!

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int i = 1;
            foreach (var person in activeStaff)
            {
                Console.WriteLine($"{i++}. {person.FIO} Подразделение: {person.POD}. Должность: {person.DOL}.");
            }

            #region 1 Table
            //var activeStaff = dbContext.CADRE_VIEW_PERSONSL.ToList().OrderBy(p=>p.FIO);

            //Console.OutputEncoding = System.Text.Encoding.UTF8;

            //int i = 1;
            //foreach (var person in activeStaff)
            //{
            //    Console.WriteLine($"{i++}. {person.FIO} Подразделение: {person.POD}. Должность: {person.DOL}."); //({person.ISN_PERSON})
            //} 
            #endregion

            Console.ReadKey();
        }
    }
}