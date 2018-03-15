using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusFactory : ILicenseStatusFactory
    {
        private readonly EmailConfig emailConfig;

        public LicenseStatusFactory(IOptions<EmailConfig> emailOptions)
        {
            this.emailConfig = emailOptions.Value;
        }

        public ILicenseStatusHandler GetInstance(LicenseStatus status)
        {
            switch (status)
            {
                case LicenseStatus.AuthPending: return new LicenseStatusAuthPendingHandler(emailConfig);
                case LicenseStatus.Pending: return new LicenseStatusPendingHandler(emailConfig);
                case LicenseStatus.Rejected: return new LicenseStatusRejectHandler(emailConfig);
                case LicenseStatus.Approved: return new LicenseStatusApproveHandler(emailConfig);
                case LicenseStatus.ApprovePending: return new LicenseStatusApprovePendingHandler(emailConfig);
                default: return null;
            }
        }
    }
}
