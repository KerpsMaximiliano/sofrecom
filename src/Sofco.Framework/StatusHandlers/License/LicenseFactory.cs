﻿namespace Sofco.Framework.StatusHandlers.License
{
    public static class LicenseFactory
    {
        public static ILicenseValidator GetInstance(int typeId)
        {
            switch (typeId)
            {
                //Reordenar / Completar
                case 1: return new HolidaysLicense();                
                case 2: 
                case 3: 
                case 4: 
                case 5:
                case 6: return new RecoveryLicense();
                case 7: return new ExamLicense();
                case 8: 
                case 9: 
                case 10: 
                case 11:
                case 12: return new CommonLicense();
                case 13: return new OthersLicense();
                case 14: return new SpecialLicense();
                case 15:
                //Licencias Nuevas
                case 18: return new PaternityDaysLicense();
                case 19: return new BirthdayLicense();
                case 20: return new FlexDaysLicense();
                case 21:
                case 22:

                default: return null;
            }
        }
    }
}
