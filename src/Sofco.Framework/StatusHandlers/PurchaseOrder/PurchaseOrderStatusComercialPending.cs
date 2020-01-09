using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
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
    public class PurchaseOrderStatusComercialPending : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailBuilder mailBuilder;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IPurchaseOrderStatusRecipientManager recipientManager;
        private readonly IUserData userData;

        private const string StatusDescription = "Pendiente Aprobación Operativa";
        private const string RejectStatusDescription = "Rechazada";
        private const string AreaDescription = "Comercial";

        public PurchaseOrderStatusComercialPending(IUnitOfWork unitOfWork, 
            IMailBuilder mailBuilder, 
            IMailSender mailSender, 
            EmailConfig emailConfig, 
            IPurchaseOrderStatusRecipientManager recipientManager,
            IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
            this.recipientManager = recipientManager;
            this.userData = userData;
        }

        public void Validate(Response response, PurchaseOrderStatusParams model, Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            if (model.MustReject && string.IsNullOrWhiteSpace(model.Comments))
            {
                response.AddError(Resources.Billing.PurchaseOrder.CommentsRequired);
            }

            var currentUser = userData.GetCurrentUser();

            var area = unitOfWork.AreaRepository.Get(purchaseOrder.AreaId.GetValueOrDefault());

            var delegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.PurchaseOrderApprovalCommercial);

            if (area == null) return;

            if (area.ResponsableUserId != currentUser.Id && !delegates.Any(x => x.UserId == area.ResponsableUserId && x.AnalyticSourceId.GetValueOrDefault() == area.Id))
            {
                response.AddError(Resources.Billing.PurchaseOrder.UserAreaWrong);
            }
        }

        public void Save(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            purchaseOrder.Status = model.MustReject ? PurchaseOrderStatus.Reject : PurchaseOrderStatus.OperativePending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage(PurchaseOrderStatusParams model)
        {
            return model.MustReject ? Resources.Billing.PurchaseOrder.RejectSuccess : Resources.Billing.PurchaseOrder.ComercialSuccess;
        }

        public void SendMail(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            var data = !model.MustReject ? CreateMailSuccess(purchaseOrder) : CreateMailReject(purchaseOrder, model.Comments);

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }

        private MailDefaultData CreateMailSuccess(Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, StatusDescription, purchaseOrder.AccountName);

            var body = string.Format(Resources.Mails.MailMessageResource.OcComercialMessage, 
                purchaseOrder.Number, 
                $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}",
                GetAnalyticsAndManagersBody(purchaseOrder),
                purchaseOrder.AccountName);

            var recipients = recipientManager.GetRecipientsOperation(purchaseOrder);

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            return data;
        }

        private MailDefaultData CreateMailReject(Domain.Models.Billing.PurchaseOrder purchaseOrder, string comments)
        {
            var ocText = $"{purchaseOrder.Number} - {purchaseOrder.AccountName}";

            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle,
                purchaseOrder.Number,
                RejectStatusDescription,
                purchaseOrder.AccountName);

            var body = string.Format(Resources.Mails.MailMessageResource.OcRejectMessage,
                ocText,
                AreaDescription,
                comments,
                $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}",
                GetAnalyticsAndManagersBody(purchaseOrder));

            var recipients = recipientManager.GetRejectCommercial(purchaseOrder);

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            return data;
        }

        private string GetAnalyticsAndManagersBody(Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var analytics = unitOfWork.AnalyticRepository.GetByPurchaseOrder(purchaseOrder.Id);

            var analyticsForBody = string.Empty;

            foreach (var analytic in analytics)
            {
                analyticsForBody = string.Concat(analyticsForBody, $"{analytic.Manager?.Name}: {analytic.Title} - {analytic.Name} <br/>");
            }

            return analyticsForBody;
        }
    }
}
