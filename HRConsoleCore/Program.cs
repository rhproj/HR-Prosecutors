using EosKadriLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace HRConsoleCore
{
    class Program
    {
        static void Main(string[] args)
        {
            #region in case of json file 
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //IConfigurationRoot configuration = builder.Build();

            ////Console.WriteLine(configuration.GetConnectionString("K092Context"));
            #endregion

            var dbContext = new K092Context();  //K092DBContext();

            var activeStaff = (from pSL in dbContext.CADRE_VIEW_PERSONSL
                              join fio in dbContext.CADRE_VIEW_FIO on
                              pSL.ISN_PERSON equals fio.ISN_PERSON
                              orderby fio.FIO
                              select new
                              {
                                  FIO = fio.FIO,
                                  DOL = pSL.DOL,
                                  POD = pSL.POD,
                                  ISN_PERSON = pSL.ISN_PERSON
                              }).Take(10);  //).ToList(); - not needed cuz it's IQuariable already wich means foreach works!

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int i = 1;
            foreach (var person in activeStaff)
            {
                Console.WriteLine($"{i++}. {person.FIO} Подразделение: {person.POD}. Должность: {person.DOL}.");
            }

        }
    }
}
