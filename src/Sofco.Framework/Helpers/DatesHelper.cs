using Sofco.Domain.Models.AllocationManagement;
using System;

namespace Sofco.Framework.Helpers
{
    public static class DatesHelper
    {
        public static string GetDateShortDescription(DateTime date)
        {
            switch (date.Month)
            {
                case 1: return $"Ene. {date.Year}";
                case 2: return $"Feb. {date.Year}";
                case 3: return $"Mar. {date.Year}";
                case 4: return $"Abr. {date.Year}";
                case 5: return $"May. {date.Year}";
                case 6: return $"Jun. {date.Year}";
                case 7: return $"Jul. {date.Year}";
                case 8: return $"Ago. {date.Year}";
                case 9: return $"Sep. {date.Year}";
                case 10: return $"Oct. {date.Year}";
                case 11: return $"Nov. {date.Year}";
                case 12: return $"Dic. {date.Year}";
                default: return string.Empty;
            }
        }

        public static string GetDateDescription(DateTime date)
        {
            switch (date.Month)
            {
                case 1: return $"Enero {date.Year}";
                case 2: return $"Febrero {date.Year}";
                case 3: return $"Marzo {date.Year}";
                case 4: return $"Abril {date.Year}";
                case 5: return $"Mayo {date.Year}";
                case 6: return $"Junio {date.Year}";
                case 7: return $"Julio {date.Year}";
                case 8: return $"Agosto {date.Year}";
                case 9: return $"Septiembre {date.Year}";
                case 10: return $"Octubre {date.Year}";
                case 11: return $"Noviembre {date.Year}";
                case 12: return $"Diciembre {date.Year}";
                default: return string.Empty;
            }
        }

        public static string GetMonthDesc(int month)
        {
            switch (month)
            {
                case 1: return "Enero";
                case 2: return "Febrero";
                case 3: return "Marzo";
                case 4: return "Abril";
                case 5: return "Mayo";
                case 6: return "Junio";
                case 7: return "Julio";
                case 8: return "Agosto";
                case 9: return "Septiembre";
                case 10: return "Octubre";
                case 11: return "Noviembre";
                case 12: return "Diciembre";
                default: return string.Empty;
            }
        }

        public static bool ValidateMonth(int month)
        {
            if (month < 1 || month > 12) return false;

            return true;
        }

        public static void SetHolydayDays(Employee employee, int daysWorked)
        {
            var daysAvg = (double)daysWorked / 25;

            daysAvg = Math.Ceiling(daysAvg);

            var days = (int)daysAvg;
            employee.HolidaysPendingByLaw = days;
            employee.HolidaysByLaw = days;

            if (days > 6)
            {
                days -= 2;
            }
            else if (days == 6)
            {
                days--;
            }

            employee.HolidaysPending = days;
        }

        public static int GetWorkedDays(Employee employee)
        {
            return new DateTime(DateTime.UtcNow.Year, 12, 31).Subtract(employee.StartDate.Date).Days + 1;
        }
    }
}
 