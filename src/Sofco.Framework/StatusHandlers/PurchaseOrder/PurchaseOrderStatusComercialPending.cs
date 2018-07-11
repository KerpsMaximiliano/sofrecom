using System;
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
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.PurchaseOrder
{
    public class PurchaseOrderStatusComercialPending : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private readonly IUserData userData;

        private const string StatusDescription = "Pendiente Aprobación Operativa";
        private const string RejectStatusDescription = "Rechazada";
        private const string AreaDescription = "Comercial";

        public PurchaseOrderStatusComercialPending(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, EmailConfig emailConfig, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
            this.userData = userData;
        }

        public void Validate(Response response, PurchaseOrderStatusParams model, Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            if (model.MustReject && string.IsNullOrWhiteSpace(model.Comments))
            {
                response.AddError(Resources.Billing.PurchaseOrder.CommentsRequired);
            }
        }

        public void Save(Model.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            purchaseOrder.Status = model.MustReject ? PurchaseOrderStatus.Reject : PurchaseOrderStatus.OperativePending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage(PurchaseOrderStatusParams model)
        {
            return model.MustReject ? Resources.Billing.PurchaseOrder.RejectSuccess : Resources.Billing.PurchaseOrder.ComercialSuccess;
        }

        public void SendMail(Model.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            var data = !model.MustReject ? CreateMailSuccess(purchaseOrder) : CreateMailReject(purchaseOrder, model.Comments);

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }

        private MailDefaultData CreateMailSuccess(Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var subjectToDaf = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, StatusDescription);
            var bodyToDaf = string.Format(Resources.Mails.MailMessageResource.OcComercialMessage, purchaseOrder.Number, $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipients = GetSuccessRecipients(purchaseOrder);

            var data = new MailDefaultData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = recipients
            };

            return data;
        }

        private MailDefaultData CreateMailReject(Model.Models.Billing.PurchaseOrder purchaseOrder, string comments)
        {
            var subjectToDaf = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, RejectStatusDescription);

            var bodyToDaf = string.Format(Resources.Mails.MailMessageResource.OcRejectMessage,
                purchaseOrder.Number,
                AreaDescription,
                comments,
                $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var mail = unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);

            var area = unitOfWork.AreaRepository.GetWithResponsable(purchaseOrder.AreaId.GetValueOrDefault());

            if (area?.ResponsableUser != null)
            {
                mail = $"{mail};{area.ResponsableUser.Email}";
            }

            var data = new MailDefaultData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = mail
            };

            return data;
        }

        private string GetSuccessRecipients(Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var analytics = unitOfWork.PurchaseOrderRepository.GetByAnalyticsWithSectors(purchaseOrder.Id);

            var users = analytics.Select(analytic => analytic.Sector?.ResponsableUser).ToList();

            if (!users.Any()) return string.Empty;

            var mails = new List<string>();

            foreach (var user in users)
            {
                mails.Add(user.Email);

                var userIds = unitOfWork.UserDelegateRepository.GetByUserId(user.Id,
                    UserDelegateType.PurchaseOrderOperation)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return string.Join(";", mails.Distinct());
        }
    }
}
