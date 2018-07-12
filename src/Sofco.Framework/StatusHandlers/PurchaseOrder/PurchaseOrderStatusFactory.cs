using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.PurchaseOrder
{
    public class PurchaseOrderStatusFactory : IPurchaseOrderStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private readonly AppSetting appSetting;

        private readonly IUserData userData;

        public PurchaseOrderStatusFactory(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, IOptions<EmailConfig> emailOptions, IOptions<AppSetting> appSettingOptions, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.userData = userData;
            this.emailConfig = emailOptions.Value;
            this.appSetting = appSettingOptions.Value;
        }

        public IPurchaseOrderStatusHandler GetInstance(PurchaseOrderStatus status)
        {
            switch (status)
            {
                case PurchaseOrderStatus.Draft: return new PurchaseOrderStatusDraft(unitOfWork, mailBuilder, mailSender, emailConfig, userData);
                case PurchaseOrderStatus.CompliancePending: return new PurchaseOrderStatusCompliancePending(unitOfWork, mailBuilder, mailSender, emailConfig, userData);
                case PurchaseOrderStatus.ComercialPending: return new PurchaseOrderStatusComercialPending(unitOfWork, mailBuilder, mailSender, emailConfig, userData);
                case PurchaseOrderStatus.OperativePending: return new PurchaseOrderStatusOperativePending(unitOfWork, mailBuilder, mailSender, emailConfig, appSetting, userData);
                case PurchaseOrderStatus.DafPending: return new PurchaseOrderStatusDafPending(unitOfWork, mailBuilder, mailSender, emailConfig, appSetting, userData);
                case PurchaseOrderStatus.Reject: return new PurchaseOrderStatusReject(unitOfWork, mailBuilder, mailSender, emailConfig);
                default: return null;
            }
        }
    }
}
