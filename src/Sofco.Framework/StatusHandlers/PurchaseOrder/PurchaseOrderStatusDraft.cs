using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Framework.StatusHandlers.PurchaseOrder
{
    public class PurchaseOrderStatusDraft : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailBuilder mailBuilder;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IPurchaseOrderStatusRecipientManager recipientManager;

        private const string StatusDescription = "Pendiente Aprobación Compliance";

        public PurchaseOrderStatusDraft(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, EmailConfig emailConfig, IPurchaseOrderStatusRecipientManager recipientManager)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
            this.recipientManager = recipientManager;
        }

        public void Validate(Response response, PurchaseOrderStatusParams model, Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            if (!purchaseOrder.FileId.HasValue)
                response.AddError(Resources.Billing.PurchaseOrder.FileRequired);
        } 

        public void Save(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            purchaseOrder.Status = PurchaseOrderStatus.CompliancePending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage(PurchaseOrderStatusParams model)
        {
            return model.MustReject ? Resources.Billing.PurchaseOrder.RejectSuccess : Resources.Billing.PurchaseOrder.DraftSuccess;
        }

        public void SendMail(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            var analytics = unitOfWork.AnalyticRepository.GetByPurchaseOrder(purchaseOrder.Id);

            var analyticsForBody = string.Empty;

            foreach (var analytic in analytics)
            {
                analyticsForBody = string.Concat(analyticsForBody, $"{analytic.Manager.Name}: {analytic.Title} - {analytic.Name} <br/>");
            }

            var ocText = $"{purchaseOrder.Number} - {purchaseOrder.AccountName}";

            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, ocText, StatusDescription, purchaseOrder.AccountName);

            var body = string.Format(Resources.Mails.MailMessageResource.OcDraftMessage, 
                ocText, 
                $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}", 
                analyticsForBody,
                purchaseOrder.AccountName);

            var recipients = recipientManager.GetRecipientsCompliance();

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            var email = mailBuilder.GetEmail(data);

            mailSender.Send(email);
        }
    }
}
