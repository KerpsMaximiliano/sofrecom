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
    }
}
