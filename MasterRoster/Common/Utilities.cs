using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterRoster.Common
{
    public class Utilities
    {
        public static void GetCurrentWeek(DateTime today, out DateTime startingMonday, out DateTime endingMonday)
        {
            int diff = today.DayOfWeek - DayOfWeek.Monday;
            if(diff < 0)
            {
                diff += 7;
            }
            // TODO: starting time can be read from Web.Config
            TimeSpan ts = new TimeSpan(5, 45, 0);
            startingMonday = today.AddDays(-1 * diff).Date + ts;
            endingMonday = today.AddDays(7 - diff).Date + ts;

        }

        public static void GetCurrentMonth(DateTime date, out DateTime firstDayOfMonth, out DateTime lastDayOfMonth)
        {
            firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        }
    }
}