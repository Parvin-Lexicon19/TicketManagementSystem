using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Data
{
    public static class DateTimeExtensions
    {
        /*Calculates work days based on priority*/
        public static DateTime SetDueDate(Priority priority)
        {
            DateTime dueDate = DateTime.Now;
            int workDays = (int)priority;
            while (workDays > 0)
            {
                dueDate = dueDate.AddDays(1);
                if (dueDate.DayOfWeek < DayOfWeek.Saturday &&
                    dueDate.DayOfWeek > DayOfWeek.Sunday &&
                    !dueDate.Date.IsHoliday())
                    workDays--;
            }
            return dueDate;
        }

        public static bool IsHoliday(this DateTime date)
        {
            /*You'd load/cache from a DB or file somewhere rather than hardcode
              Also the list should be updated every year*/
            DateTime[] holidays =
                       new DateTime[] {
                       new DateTime(2020,01,01),
                       new DateTime(2020,01,06),
                       new DateTime(2020,04,10),
                       new DateTime(2020,04,12),
                       new DateTime(2020,04,13),
                       new DateTime(2020,05,01),
                       new DateTime(2020,05,21),
                       new DateTime(2020,05,31),
                       new DateTime(2020,06,06),
                       new DateTime(2020,06,20),                       
                       new DateTime(2020,10,31),
                       new DateTime(2020,12,25),
                       new DateTime(2020,12,26)
                       };

            return holidays.Contains(date.Date);
        }
    }
}
