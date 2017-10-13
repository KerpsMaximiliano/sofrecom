﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Model;
using Sofco.Resources;
using Sofco.Core.Config;
using Sofco.Service.Settings.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class SolfacJobService : ISolfacJobService
    {
        const string DateFormat = "dd/MM/yyyy";
        const string Subject = "HITOS sin Solfac";

        private int DaysToExpire = 5;

        private readonly ISolfacRepository solfacRepository;
        private readonly ICrmInvoiceService crmInvoiceService;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly SolfacJobSetting solfacJobSetting;

        public SolfacJobService(ISolfacRepository solfacRepository,
            ICrmInvoiceService crmInvoiceService,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions,
            IOptions<SolfacJobSetting> solfacJobOptions)
        {
            this.solfacRepository = solfacRepository;
            this.crmInvoiceService = crmInvoiceService;
            this.mailSender = mailSender;
            emailConfig = emailConfigOptions.Value;
            solfacJobSetting = solfacJobOptions.Value;
            DaysToExpire = solfacJobSetting.DaysToExpire;
        }

        public void SendHitosNotfications()
        {
            var hitos = GetHitosWithoutSolfac();

            var emails = GetEmailByHitos(hitos);

            mailSender.Send(emails);
        }

        private List<CrmHito> GetHitosWithoutSolfac()
        {
            var crmHitosResult = crmInvoiceService.GetHitosToExpire(DaysToExpire);

            var crmHitosList = crmHitosResult.Data.ToList();

            var crmHitosIdList = crmHitosList.Select(s => s.Id).ToList();

            var hitos = solfacRepository.GetHitosByExternalIds(crmHitosIdList);

            var hitoExternalIds = hitos.Select(s => new Guid(s.ExternalHitoId));

            var resultList = crmHitosList.Where(s => !hitoExternalIds.Contains(s.Id)).ToList();

            return resultList;
        }

        protected List<Email> GetEmailByHitos(List<CrmHito> hitos)
        {
            var groupedList = GroupByManager(hitos);

            var result = new List<Email>();

            foreach (var item in groupedList)
            {
                if (item.Key == null) continue;
                result.Add(new Email
                {
                    Subject = Subject,
                    Recipient = item.Key,
                    Body = BuildBody(item.Value)
                });
            }

            return result;
        }

        private Dictionary<string, List<CrmHito>> GroupByManager(List<CrmHito> hitos)
        {
            var list = new Dictionary<string, List<CrmHito>>();

            foreach (var item in hitos)
            {
                var key = item.ManagerMail;
                if (key == null) continue;
                if (list.ContainsKey(key))
                {
                    list[key].Add(item);
                } else
                {
                    list[key] = new List<CrmHito> { item };
                }
            }

            return list;
        }

        private string BuildBody(List<CrmHito> hitos)
        {
            var template = MailResource.HitosWithoutSolfac;

            var content = new StringBuilder();

            foreach(var item in hitos)
            {
                var link = $"{emailConfig.SiteUrl}/billing/"+
                    $"customers/{item.CustomerId}"+
                    $"/services/{item.ServiceId}"+
                    $"/projects/{item.ProjectId}";

                content.AppendLine($"<li><a href='{link}'>{item.Name} - {item.ProjectName} - {item.ScheduledDate.ToString(DateFormat)}</a>");
            }

            var body = template.Replace("{content}", $"<ul>{content.ToString()}</ul>");

            body = body.Replace("{siteUrl}", emailConfig.SiteUrl);


            return body;
        }
    }
}
