using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusFactory : ILicenseStatusFactory
    {
        public ILicenseStatusHandler GetInstance(LicenseStatus status)
        {
            switch (status)
            {
                case LicenseStatus.AuthPending: return new LicenseStatusAuthPendingHandler();
                case LicenseStatus.Pending: return new LicenseStatusPendingHandler();
                case LicenseStatus.Rejected: return new LicenseStatusRejectHandler();
                case LicenseStatus.Approved: return new LicenseStatusApproveHandler();
                default: return null;
            }
        }
    }
}
