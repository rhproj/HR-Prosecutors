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

            var activeStaff = dbContext.CADRE_VIEW_PERSONSL.ToList().OrderBy(p=>p.FIO);

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int i = 1;
            foreach (var person in activeStaff)
            {
                Console.WriteLine($"{i++}. {person.FIO} Подразделение: {person.POD}. Должность: {person.DOL}."); //({person.ISN_PERSON})
            }

            Console.ReadKey();
        }
    }
}
