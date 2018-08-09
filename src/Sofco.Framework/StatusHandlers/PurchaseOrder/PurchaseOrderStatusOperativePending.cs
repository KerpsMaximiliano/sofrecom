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
    public class PurchaseOrderStatusOperativePending : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailBuilder mailBuilder;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IPurchaseOrderStatusRecipientManager recipientManager;
        private readonly IUserData userData;

        private const string StatusDescription = "Pendiente Aprobación DAF";
        private const string RejectStatusDescription = "Rechazada";
        private const string AreaDescription = "Operativa";

        public PurchaseOrderStatusOperativePending(IUnitOfWork unitOfWork, 
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

            var analytics = unitOfWork.AnalyticRepository.GetByPurchaseOrder(purchaseOrder.Id);

            if (analytics.Any())
            {
                var sectorIds = analytics.Select(x => x.SectorId).ToList();
                var sectors = unitOfWork.SectorRepository.Get(sectorIds);
                var responsables = sectors.Select(x => x.ResponsableUserId);

                if (!responsables.Contains(userData.GetCurrentUser().Id))
                {
                    response.AddError(Resources.Billing.PurchaseOrder.UserSectorWrong);
                }
            }
        }

        public void Save(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            purchaseOrder.Status = model.MustReject ? PurchaseOrderStatus.Reject : PurchaseOrderStatus.DafPending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage(PurchaseOrderStatusParams model)
        {
            return model.MustReject ? Resources.Billing.PurchaseOrder.RejectSuccess : Resources.Billing.PurchaseOrder.OperativeSuccess;
        }

        public void SendMail(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            var data = !model.MustReject ? CreateMailSuccess(purchaseOrder) : CreateMailReject(purchaseOrder, model.Comments);

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }

        private MailDefaultData CreateMailSuccess(Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var subjectToDaf = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, StatusDescription);
            var bodyToDaf = string.Format(Resources.Mails.MailMessageResource.OcOperativeMessage, purchaseOrder.Number, $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipients = recipientManager.GetRecipientsDaf();

            var data = new MailDefaultData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = recipients
            };

            return data;
        }

        private MailDefaultData CreateMailReject(Domain.Models.Billing.PurchaseOrder purchaseOrder, string comments)
        {
            var ocText = $"{purchaseOrder.Number} - {purchaseOrder.ClientExternalName}";

            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, ocText, RejectStatusDescription);

            var body = string.Format(Resources.Mails.MailMessageResource.OcRejectMessage,
                ocText,
                AreaDescription,
                comments,
                $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipients = recipientManager.GetRejectOperation(purchaseOrder);

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            return data;
        }
    }
}
