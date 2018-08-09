using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;

namespace Sofco.Framework.StatusHandlers.PurchaseOrder
{
    public class PurchaseOrderStatusFactory : IPurchaseOrderStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private readonly IPurchaseOrderStatusRecipientManager recipientManager;

        private readonly IUserData userData;

        public PurchaseOrderStatusFactory(IUnitOfWork unitOfWork, 
            IMailBuilder mailBuilder, 
            IMailSender mailSender, 
            IOptions<EmailConfig> emailOptions, 
            IPurchaseOrderStatusRecipientManager recipientManager,
            IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.recipientManager = recipientManager;
            emailConfig = emailOptions.Value;
            this.userData = userData;
        }

        public IPurchaseOrderStatusHandler GetInstance(PurchaseOrderStatus status)
        {
            switch (status)
            {
                case PurchaseOrderStatus.Draft: return new PurchaseOrderStatusDraft(unitOfWork, mailBuilder, mailSender, emailConfig, recipientManager);
                case PurchaseOrderStatus.CompliancePending: return new PurchaseOrderStatusCompliancePending(unitOfWork, mailBuilder, mailSender, emailConfig, recipientManager);
                case PurchaseOrderStatus.ComercialPending: return new PurchaseOrderStatusComercialPending(unitOfWork, mailBuilder, mailSender, emailConfig, recipientManager, userData);
                case PurchaseOrderStatus.OperativePending: return new PurchaseOrderStatusOperativePending(unitOfWork, mailBuilder, mailSender, emailConfig, recipientManager, userData);
                case PurchaseOrderStatus.DafPending: return new PurchaseOrderStatusDafPending(unitOfWork, mailBuilder, mailSender, emailConfig, recipientManager);
                case PurchaseOrderStatus.Reject: return new PurchaseOrderStatusReject(unitOfWork, mailBuilder, mailSender, emailConfig, recipientManager);
                default: return null;
            }
        }
    }
}
