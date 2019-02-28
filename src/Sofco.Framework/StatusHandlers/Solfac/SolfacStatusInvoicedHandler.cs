using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;

using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusInvoicedHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoicingMilestoneService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public SolfacStatusInvoicedHandler(IUnitOfWork unitOfWork, ICrmInvoicingMilestoneService crmInvoiceService, IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response Validate(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.InvoicePending)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateInvoiceCode(parameters, unitOfWork.SolfacRepository, response, solfac.InvoiceCode);
            SolfacValidationHelper.ValidateInvoiceDate(parameters, response, solfac);
            
            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusInvoicedMessage, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusInvoicedTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipient(Domain.Models.Billing.Solfac solfac)
        {
            return solfac.UserApplicant.Email;
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.InvoicedSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.Billed;
        }

        public void SaveStatus(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Domain.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status, InvoiceCode = parameters.InvoiceCode, InvoiceDate = parameters.InvoiceDate };
            unitOfWork.SolfacRepository.UpdateStatusAndInvoice(solfacToModif);
            solfac.InvoiceDate = parameters.InvoiceDate;
            solfac.InvoiceCode = parameters.InvoiceCode;

            if (solfac.PurchaseOrder == null) return;

            var detail = solfac.PurchaseOrder.AmmountDetails.SingleOrDefault(x => x.CurrencyId == solfac.CurrencyId);

            if (detail != null)
            {
                var oldBalance = detail.Balance;

                detail.Balance = detail.Balance - solfac.TotalAmount;
                unitOfWork.PurchaseOrderRepository.UpdateBalance(detail);

                if (oldBalance > 0 && detail.Balance <= 0)
                {
                    solfac.PurchaseOrder.Status = PurchaseOrderStatus.Consumed;
                    unitOfWork.PurchaseOrderRepository.UpdateStatus(solfac.PurchaseOrder);
                    SendMailForOcConsumed(solfac.PurchaseOrder);
                }
            }
        }

        private void SendMailForOcConsumed(Domain.Models.Billing.PurchaseOrder purchaseOrder)
        {
            var analytics = unitOfWork.AnalyticRepository.GetByPurchaseOrder(purchaseOrder.Id);

            var recipients = new List<string>();

            foreach (var analytic in analytics)
            {
                if (analytic.Manager != null)
                {
                    recipients.Add(analytic.Manager.Email);
                }
            }

            if (!recipients.Any()) return;

            var subject = string.Format(Resources.Mails.MailSubjectResource.PurchaseOrderConsumed, purchaseOrder.Number);
            var body = string.Format(Resources.Mails.MailMessageResource.PurchaseOrderConsumed, purchaseOrder.Number);

            var data = new SolfacStatusData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }

        public void UpdateHitos(ICollection<string> hitos, Domain.Models.Billing.Solfac solfac)
        {
            crmInvoiceService.UpdateStatusAndInvoiceDateAndNumber(hitos.ToList(), GetHitoStatus(), solfac.InvoiceDate.GetValueOrDefault(), solfac.InvoiceCode);
        }

        public void SendMail(Domain.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subject = GetSubjectMail(solfac);
            var body = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipient = GetRecipient(solfac);

            var data = new SolfacStatusData
            {
                Title = subject,
                Message = body,
                Recipient = recipient
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
