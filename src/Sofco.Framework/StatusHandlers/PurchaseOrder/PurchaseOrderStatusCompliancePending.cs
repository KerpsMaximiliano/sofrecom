using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.PurchaseOrder
{
    public class PurchaseOrderStatusCompliancePending : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private const string StatusDescription = "Pendiente Aprobación Comercial";

        public PurchaseOrderStatusCompliancePending(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, EmailConfig emailConfig)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
        }

        public void Validate(Response response, PurchaseOrderStatusParams model, Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
        }

        public void Save(Model.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            purchaseOrder.Status = PurchaseOrderStatus.ComercialPending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.PurchaseOrder.ComplianceSuccess;
        }

        public void SendMail(Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var subjectToDaf = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, StatusDescription);
            var bodyToDaf = string.Format(Resources.Mails.MailMessageResource.OcComplianceMessage, purchaseOrder.Number, $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipientsToDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.ComplianceCode);

            var data = new MailDefaultData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = recipientsToDaf
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
