using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusFactory : ILicenseStatusFactory
    {
        private readonly EmailConfig emailConfig;

        private readonly ILicenseApproverManager licenseApproverManager;

        public LicenseStatusFactory(IOptions<EmailConfig> emailOptions, ILicenseApproverManager licenseApproverManager)
        {
            this.licenseApproverManager = licenseApproverManager;
            this.emailConfig = emailOptions.Value;
        }

        public ILicenseStatusHandler GetInstance(LicenseStatus status)
        {
            switch (status)
            {
                case LicenseStatus.AuthPending: return new LicenseStatusAuthPendingHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.Pending: return new LicenseStatusPendingHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.Rejected: return new LicenseStatusRejectHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.Approved: return new LicenseStatusApproveHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.ApprovePending: return new LicenseStatusApprovePendingHandler(emailConfig, licenseApproverManager);
                case LicenseStatus.Cancelled: return new LicenseStatusCancelledHandler(emailConfig, licenseApproverManager);
                default: return null;
            }
        }
    }
}
