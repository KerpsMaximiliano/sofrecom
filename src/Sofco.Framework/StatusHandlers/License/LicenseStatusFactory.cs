using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;
using Sofco.Framework.Managers.UserApprovers;

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
                case LicenseStatus.Pending: return new LicenseStatusPendingHandler(emailConfig);
                case LicenseStatus.Rejected: return new LicenseStatusRejectHandler(emailConfig);
                case LicenseStatus.Approved: return new LicenseStatusApproveHandler(emailConfig);
                case LicenseStatus.ApprovePending: return new LicenseStatusApprovePendingHandler(emailConfig);
                case LicenseStatus.Cancelled: return new LicenseStatusCancelledHandler(emailConfig);
                default: return null;
            }
        }
    }
}
