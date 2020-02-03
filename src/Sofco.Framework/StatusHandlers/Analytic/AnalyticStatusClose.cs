﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Domain.Enums;
using Sofco.Framework.MailData;

using Sofco.Domain.Utils;
using Sofco.Resources.Mails;

namespace Sofco.Framework.StatusHandlers.Analytic
{
    public static class AnalyticStatusClose
    {
        public static void Save(Domain.Models.AllocationManagement.Analytic analytic, IUnitOfWork unitOfWork, Response response, AnalyticStatus status,
            string userName)
        {
            unitOfWork.AllocationRepository.RemoveAllocationByAnalytic(analytic.Id, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));

            analytic.Status = status;
            analytic.ClosedBy = userName;
            analytic.ClosedAt = DateTime.UtcNow;
            unitOfWork.AnalyticRepository.Close(analytic);

            unitOfWork.Save();

            response.AddSuccess(status == AnalyticStatus.Close ? Resources.AllocationManagement.Analytic.CloseSuccess : Resources.AllocationManagement.Analytic.CloseForExpensesSuccess);
        }

        public static void SendMail(Response response, Domain.Models.AllocationManagement.Analytic analytic, EmailConfig emailConfig, IMailSender mailSender, IUnitOfWork unitOfWork, IMailBuilder mailBuilder)
        {
            var recipients = new List<string>();

            var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);
            //var mailDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);
            var mailCalidad = unitOfWork.GroupRepository.GetEmail(emailConfig.QualityCode);

            //recipients.AddRange(new[] { mailPmo, mailRrhh, mailDaf, mailCalidad });

            recipients.AddRange(new[] {
                mailPmo,
                "jberraz@sofrecom.com.ar",
                "iaybar@sofrecom.com.ar",
                "gperugorria@sofrecom.com.ar",
                "mscovello@sofrecom.com.ar",
                "fgiani@sofrecom.com.ar",
                "NMiguez@sofrecom.com.ar",
                "CPoquet@sofrecom.com.ar",
                mailRrhh,
                mailCalidad });

            var manager = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.ManagerId);
            var seller = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.CommercialManagerId);
            var sector = unitOfWork.SectorRepository.Get(analytic.SectorId);
            var director = unitOfWork.UserRepository.Get(sector.ResponsableUserId);

            if (manager != null) recipients.Add(manager.Email);
            if (seller != null) recipients.Add(seller.Email);
            if (director != null) recipients.Add(director.Email);

            var title = analytic.Status == AnalyticStatus.Close ? MailSubjectResource.CloseAnalytic : MailSubjectResource.CloseForExpensesAnalytic;
            var message = analytic.Status == AnalyticStatus.Close ? MailMessageResource.CloseAnalytic : MailMessageResource.CloseForExpensesAnalytic;

            var data = new CloseAnalyticData
            {
                Title = string.Format(title, analytic.AccountName),
                Message = string.Format(message, $"{analytic.Title} - {analytic.Name}", analytic.ServiceName, $"{emailConfig.SiteUrl}contracts/analytics/{analytic.Id}/view"),
                Recipients = recipients.Distinct().ToList()
            };

            var email = mailBuilder.GetEmail(data);

            mailSender.Send(email);
        }
    }
}
