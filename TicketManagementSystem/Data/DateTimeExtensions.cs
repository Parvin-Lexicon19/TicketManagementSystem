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
              Also the list should be updated for years later than 2030
              And past holidays may be removed every year*/

            DateTime[] holidays =
                       new DateTime[] {
                       /*Dates caught from https://www.kalender.se/helgdagar/valj */
                       /*2020*/    
                       new DateTime(2020,01,01), new DateTime(2020,01,06), new DateTime(2020,04,10), new DateTime(2020,04,12),
                       new DateTime(2020,04,13), new DateTime(2020,05,01), new DateTime(2020,05,21), new DateTime(2020,05,31),
                       new DateTime(2020,06,06), new DateTime(2020,06,20), new DateTime(2020,10,31), new DateTime(2020,12,25),
                       new DateTime(2020,12,26),

                       /*2021*/
                       new DateTime(2021,01,01), new DateTime(2021,01,06), new DateTime(2021,04,02), new DateTime(2021,04,04),
                       new DateTime(2021,04,05), new DateTime(2021,05,01), new DateTime(2021,05,13), new DateTime(2021,05,23),
                       new DateTime(2021,06,06), new DateTime(2021,06,26), new DateTime(2021,11,06), new DateTime(2021,12,25),
                       new DateTime(2021,12,26),

                       /*2022*/
                       new DateTime(2022,01,01), new DateTime(2022,01,06), new DateTime(2022,04,15), new DateTime(2022,04,17),
                       new DateTime(2022,04,18), new DateTime(2022,05,01), new DateTime(2022,05,26), new DateTime(2022,06,05),
                       new DateTime(2022,06,06), new DateTime(2022,06,25), new DateTime(2022,11,05), new DateTime(2022,12,25),
                       new DateTime(2022,12,26),

                       /*2023*/
                       new DateTime(2023,01,01), new DateTime(2023,01,06), new DateTime(2023,04,07), new DateTime(2023,04,09),
                       new DateTime(2023,04,10), new DateTime(2023,05,01), new DateTime(2023,05,18), new DateTime(2023,05,28),
                       new DateTime(2023,06,06), new DateTime(2023,06,24), new DateTime(2023,11,04), new DateTime(2023,12,25),
                       new DateTime(2023,12,26),

                       /*2024*/
                       new DateTime(2024,01,01), new DateTime(2024,01,06), new DateTime(2024,03,29), new DateTime(2024,03,31),
                       new DateTime(2024,04,01), new DateTime(2024,05,01), new DateTime(2024,05,09), new DateTime(2024,05,19),
                       new DateTime(2024,06,06), new DateTime(2024,06,22), new DateTime(2024,11,02), new DateTime(2024,12,25),
                       new DateTime(2024,12,26),

                       /*2025*/
                       new DateTime(2025,01,01), new DateTime(2025,01,06), new DateTime(2025,04,18), new DateTime(2025,04,20),
                       new DateTime(2025,04,21), new DateTime(2025,05,01), new DateTime(2025,05,29), new DateTime(2025,06,06),
                       new DateTime(2025,06,08), new DateTime(2025,06,21), new DateTime(2025,11,01), new DateTime(2025,12,25),
                       new DateTime(2025,12,26),

                       /*2026*/
                       new DateTime(2026,01,01), new DateTime(2026,01,06), new DateTime(2026,04,03), new DateTime(2026,04,05),
                       new DateTime(2026,04,06), new DateTime(2026,05,01), new DateTime(2026,05,14), new DateTime(2026,05,24),
                       new DateTime(2026,06,06), new DateTime(2026,06,20), new DateTime(2026,10,31), new DateTime(2026,12,25),
                       new DateTime(2026,12,26),

                       /*2027*/
                       new DateTime(2027,01,01), new DateTime(2027,01,06), new DateTime(2027,03,26), new DateTime(2027,03,28),
                       new DateTime(2027,03,29), new DateTime(2027,05,01), new DateTime(2027,05,06), new DateTime(2027,05,16),
                       new DateTime(2027,06,06), new DateTime(2027,06,26), new DateTime(2027,11,06), new DateTime(2027,12,25),
                       new DateTime(2027,12,26),

                       /*2028*/
                       new DateTime(2028,01,01), new DateTime(2028,01,06), new DateTime(2028,04,14), new DateTime(2028,04,16),
                       new DateTime(2028,04,17), new DateTime(2028,05,01), new DateTime(2028,05,25), new DateTime(2028,06,04),
                       new DateTime(2028,06,06), new DateTime(2028,06,24), new DateTime(2028,11,04), new DateTime(2028,12,25),
                       new DateTime(2028,12,26),

                       /*2029*/
                       new DateTime(2029,01,01), new DateTime(2029,01,06), new DateTime(2029,03,30), new DateTime(2029,04,01),
                       new DateTime(2029,04,02), new DateTime(2029,05,01), new DateTime(2029,05,10), new DateTime(2029,05,20),
                       new DateTime(2029,06,06), new DateTime(2029,06,23), new DateTime(2029,11,03), new DateTime(2029,12,25),
                       new DateTime(2029,12,26),

                       /*2030*/
                       new DateTime(2030,01,01), new DateTime(2030,01,06), new DateTime(2030,04,19), new DateTime(2030,04,21),
                       new DateTime(2030,04,22), new DateTime(2030,05,01), new DateTime(2030,05,30), new DateTime(2030,06,06),
                       new DateTime(2030,06,09), new DateTime(2030,06,22), new DateTime(2030,11,02), new DateTime(2030,12,25),
                       new DateTime(2030,12,26)
                       };

            return holidays.Contains(date.Date);
        }
    }
}
