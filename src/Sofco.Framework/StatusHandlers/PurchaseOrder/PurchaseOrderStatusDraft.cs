using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.PurchaseOrder
{
    public class PurchaseOrderStatusDraft : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailBuilder mailBuilder;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IUserData userData;

        private const string StatusDescription = "Pendiente Aprobación Compliance";

        public PurchaseOrderStatusDraft(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, EmailConfig emailConfig, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
            this.userData = userData;
        }

        public void Validate(Response response, PurchaseOrderStatusParams model, Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            if (!purchaseOrder.FileId.HasValue)
                response.AddError(Resources.Billing.PurchaseOrder.FileRequired);
        }

        public void Save(Model.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            purchaseOrder.Status = PurchaseOrderStatus.CompliancePending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage(PurchaseOrderStatusParams model)
        {
            return model.MustReject ? Resources.Billing.PurchaseOrder.RejectSuccess : Resources.Billing.PurchaseOrder.DraftSuccess;
        }

        public void SendMail(Model.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, StatusDescription);

            var body = string.Format(Resources.Mails.MailMessageResource.OcDraftMessage, purchaseOrder.Number, $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipients = GetRecipients();

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            var email = mailBuilder.GetEmail(data);

            mailSender.Send(email);
        }

        private string GetRecipients()
        {
            var mails = new List<string> { unitOfWork.GroupRepository.GetEmail(emailConfig.ComplianceCode) };

            var users = unitOfWork.UserRepository.GetByGroup(emailConfig.ComplianceCode);

            mails.AddRange(users.Select(s => s.Email).ToList());

            foreach (var user in users)
            {
                var userIds = unitOfWork.UserDelegateRepository.GetByUserId(user.Id,
                        UserDelegateType.PurchaseOrderCompliance)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return string.Join(";", mails.Distinct());
        }
    }
}
