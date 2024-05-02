using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inewi_Console.Entities
{
    internal class FreeDays
    {
        Dictionary<DateTime, string> immovableFreeDays = new Dictionary<DateTime, string>
        {
            { new DateTime(2000, 1, 1), "Solemnity of Mary, Mother of God / New Year" },
            { new DateTime(2000, 1, 6), "Epiphany / Three Kings' Day" },
            { new DateTime(2000, 5, 1), "May Day / Labor Day" },
            { new DateTime(2000, 5, 3), "Consitution Day / Mary the Queen of Poland Feast" },
            { new DateTime(2000, 8, 15), "Assumption of the Blessed Virgin Mary" },
            { new DateTime(2000, 11, 1), "All Saints Feast" },
            { new DateTime(2000, 11, 11), "Independence Day" },
            { new DateTime(2000, 12, 25), "Christmas" },
            { new DateTime(2000, 12, 26), "Christmas 2nd Day /  St. Stephen's Day / Boxing Day" }
        };

        public Dictionary<DateTime, string> GetFreeDaysOfYear(int year)
        {
            Dictionary<DateTime, string> freeDaysOfYear = new();

            foreach (KeyValuePair<DateTime, string>  freeDay in immovableFreeDays)
            {
                DateTime newDay = new DateTime(year, freeDay.Key.Month, freeDay.Key.Day);
                freeDaysOfYear[newDay] = freeDay.Value;
            }
            
            return freeDaysOfYear;
        }
    }
}
