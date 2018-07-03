﻿using System.Collections.Generic;
using System.Linq;
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
    public class PurchaseOrderStatusOperativePending : IPurchaseOrderStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        private const string StatusDescription = "Pendiente Aprobación DAF";
        private const string RejectStatusDescription = "Rechazada";
        private const string AreaDescription = "Operativa";

        public PurchaseOrderStatusOperativePending(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, EmailConfig emailConfig)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.emailConfig = emailConfig;
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
            purchaseOrder.Status = model.MustReject ? PurchaseOrderStatus.Reject : PurchaseOrderStatus.DafPending;
            unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
        }

        public string GetSuccessMessage(PurchaseOrderStatusParams model)
        {
            return model.MustReject ? Resources.Billing.PurchaseOrder.RejectSuccess : Resources.Billing.PurchaseOrder.OperativeSuccess;
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
            var bodyToDaf = string.Format(Resources.Mails.MailMessageResource.OcOperativeMessage, purchaseOrder.Number, $"{emailConfig.SiteUrl}billing/purchaseOrders/{purchaseOrder.Id}");

            var recipientsToDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);

            var data = new MailDefaultData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = recipientsToDaf
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

            var mails = new List<string>();

            mails.Add(unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode));

            var area = unitOfWork.AreaRepository.GetWithResponsable(purchaseOrder.AreaId.GetValueOrDefault());

            if (area?.ResponsableUser != null)
                mails.Add(area.ResponsableUser.Email);

            var analytics = unitOfWork.PurchaseOrderRepository.GetByAnalyticsWithSectors(purchaseOrder.Id);

            foreach (var analytic in analytics)
            {
                mails.Add(analytic.Sector?.ResponsableUser?.Email);
            }

            var data = new MailDefaultData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = string.Join(";", mails.Distinct())
            };

            return data;
        }
    }
}
