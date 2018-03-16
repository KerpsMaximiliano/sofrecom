namespace Sofco.Framework.StatusHandlers.License
{
    public static class LicenseFactory
    {
        public static ILicenseValidator GetInstance(int typeId)
        {
            switch (typeId)
            {
                case 1: return new HolidaysLicense();
                case 2: 
                case 3: 
                case 4: 
                case 5: 
                case 6: 
                case 8: 
                case 9: 
                case 10: 
                case 11:
                case 14:
                case 15:
                case 12: return new CommonLicense();
                case 7: return new ExamLicense();
                case 13: return new OthersLicense();

                default: return null;
            }
        }
    }
}
