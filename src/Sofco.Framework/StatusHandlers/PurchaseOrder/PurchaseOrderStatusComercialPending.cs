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
            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, StatusDescription);

            var body = string.Format(Resources.Mails.MailMessageResource.OcComercialMessage, purchaseOrder.Number, $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipients = GetSuccessRecipients(purchaseOrder);

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            return data;
        }

        private MailDefaultData CreateMailReject(Model.Models.Billing.PurchaseOrder purchaseOrder, string comments)
        {
            var subject = string.Format(Resources.Mails.MailSubjectResource.OcProcessTitle, purchaseOrder.Number, RejectStatusDescription);

            var body = string.Format(Resources.Mails.MailMessageResource.OcRejectMessage,
                purchaseOrder.Number,
                AreaDescription,
                comments,
                $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipients = GetRejectRecipients(purchaseOrder);

            var data = new MailDefaultData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            return data;
        }

        private string GetRejectRecipients(Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var mails = new List<string>{ unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode) };

            var cdgUsers = unitOfWork.UserRepository.GetByGroup(emailConfig.CdgCode);

            mails.AddRange(cdgUsers.Select(s => s.Email).ToList());

            var area = unitOfWork.AreaRepository.GetWithResponsable(purchaseOrder.AreaId.GetValueOrDefault());

            var responsableUser = area.ResponsableUser;

            mails.Add(responsableUser.Email);

            var areaUserIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderCommercial,
                    responsableUser.Id)
                .Select(s => s.UserId);

            mails.AddRange(areaUserIds.Select(userId => userData.GetById(userId))
                .Select(delegated => delegated.Email));

            return string.Join(";", mails.Distinct());
        }

        private string GetSuccessRecipients(Model.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var mails = new List<string>();

            var analytics = unitOfWork.PurchaseOrderRepository.GetByAnalyticsWithSectors(purchaseOrder.Id);

            var users = analytics.Select(analytic => analytic.Sector?.ResponsableUser).ToList();

            mails.AddRange(users.Select(s => s.Email).ToList());

            foreach (var user in users)
            {
                var userIds = unitOfWork.UserDelegateRepository.GetByTypeAndSourceId(UserDelegateType.PurchaseOrderOperation,
                        user.Id)
                    .Select(s => s.UserId);

                mails.AddRange(userIds.Select(userId => userData.GetById(userId))
                    .Select(delegated => delegated.Email));
            }

            return string.Join(";", mails.Distinct());
        }
    }
}
