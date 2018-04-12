using Sofco.Model.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface ILicenseStatusFactory
    {
        ILicenseStatusHandler GetInstance(LicenseStatus status);
    }
}
