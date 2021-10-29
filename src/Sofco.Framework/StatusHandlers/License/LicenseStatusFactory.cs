using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusFactory : ILicenseStatusFactory
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        private readonly IUserData userData;

        public LicenseStatusFactory(IOptions<EmailConfig> emailOptions, ILicenseApproverManager licenseApproverManager, IUserData userData)
        {
            this.licenseApproverManager = licenseApproverManager;
            this.emailConfig = emailOptions.Value;
            this.userData = userData;
        }

        public ILicenseStatusHandler GetInstance(LicenseStatus status)
        {
            switch (status)
            {
                case LicenseStatus.AuthPending: return new LicenseStatusAuthPendingHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.Pending: return new LicenseStatusPendingHandler(emailConfig, licenseApproverManager, userData);
                case LicenseStatus.Rejected: return new LicenseStatusRejectHandler(emailConfig, licenseApproverManager, userData);
                case LicenseStatus.Approved: return new LicenseStatusApproveHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.ApprovePending: return new LicenseStatusApprovePendingHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.Cancelled: return new LicenseStatusCancelledHandler(emailConfig, licenseApproverManager, userData);
                default: return null;
            }
        }
    }
}
