using System;

namespace Sofco.Repository.Rh.Settings
{
    public class RhSetting
    {
        public static DateTime TigerDateTimeMinValue { get; } = new DateTime(1900, 1, 1, 0, 0, 0);

        public string TigerSchema { get; set; }

        public string RhproSchema { get; set; }
    }
}
