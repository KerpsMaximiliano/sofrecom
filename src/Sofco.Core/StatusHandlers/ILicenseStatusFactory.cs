using Sofco.Domain.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface ILicenseStatusFactory
    {
        ILicenseStatusHandler GetInstance(LicenseStatus status);
    }
}
