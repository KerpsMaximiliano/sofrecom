﻿using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    /// <summary>
    /// Estado para enviar a la DAF
    /// </summary>
    public class SolfacStatusInvoicePendingHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoicingMilestoneService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public SolfacStatusInvoicePendingHandler(IUnitOfWork unitOfWork, ICrmInvoicingMilestoneService crmInvoiceService, IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response Validate(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.PendingByManagementControl)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }
            
            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusInvoicePendingMessage, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusInvoicePendingTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipient(EmailConfig emailConfig)
        {
            return unitOfWork.GroupRepository.GetEmail(emailConfig.GafCode);
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.InvoicePendingSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.ToBeBilled;
        }

        public void SaveStatus(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Domain.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);
        }

        public void UpdateHitos(ICollection<string> hitos, Domain.Models.Billing.Solfac solfac)
        {
            crmInvoiceService.UpdateStatus(hitos.ToList(), GetHitoStatus());
        }

        public void SendMail(Domain.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subject = GetSubjectMail(solfac);
            var body = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipient = GetRecipient(emailConfig);

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
