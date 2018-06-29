﻿using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusManagementControlRejectedHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public SolfacStatusManagementControlRejectedHandler(IUnitOfWork unitOfWork, ICrmInvoiceService crmInvoiceService, IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        private string mailBody = Resources.Mails.MailMessageResource.SolfacStatusManagementControlRejectedMessage;

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.PendingByManagementControl)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateComments(parameters, response);

            if (!response.HasErrors())
            {
                mailBody = mailBody.Replace("*", parameters.Comment);
            }
            
            return response;
        }

        private string GetBodyMail(Model.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(mailBody, link);
        }

        private string GetSubjectMail(Model.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusManagementControlRejectedTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipients(Model.Models.Billing.Solfac solfac, string mailCdg)
        {
            return $"{solfac.UserApplicant.Email};{mailCdg}";
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.ManagementControlRejectedSuccess;
        }

        public HitoStatus GetHitoStatus()
        {
            return HitoStatus.Pending;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);

            if (solfac.PurchaseOrder == null) return;

            var detail = solfac.PurchaseOrder.AmmountDetails.SingleOrDefault(x => x.CurrencyId == solfac.CurrencyId);

            if (detail != null)
            {
                var oldBalance = detail.Balance;
                detail.Balance = detail.Balance + solfac.TotalAmount;
                unitOfWork.PurchaseOrderRepository.UpdateBalance(detail);

                if (oldBalance <= 0 && detail.Balance > 0)
                {
                    detail.PurchaseOrder.Status = PurchaseOrderStatus.Valid;
                    unitOfWork.PurchaseOrderRepository.UpdateStatus(detail.PurchaseOrder);
                }
            }
        }

        public void UpdateHitos(ICollection<string> hitos, Model.Models.Billing.Solfac solfac, string url)
        {
            crmInvoiceService.UpdateHitosStatusAndPurchaseOrder(hitos.ToList(), GetHitoStatus(), solfac.PurchaseOrder.Number);
        }

        public void SendMail(Model.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var mailCdg = unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);

            var subject = GetSubjectMail(solfac);
            var body = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipients = GetRecipients(solfac, mailCdg);

            var data = new SolfacStatusData
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
