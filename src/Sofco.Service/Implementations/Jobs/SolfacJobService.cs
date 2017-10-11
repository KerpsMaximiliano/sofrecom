using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Model;
using Sofco.Resources;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;

namespace Sofco.Service.Implementations.Jobs
{
    public class SolfacJobService : ISolfacJobService
    {
        const int DaysToExpire = 5;
        const string DateFormat = "dd/MM/yyyy";
        const string Subject = "HITOS sin Solfac";

        private readonly ISolfacRepository solfacRepository;
        private readonly ICrmInvoiceService crmInvoiceService;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;

        public SolfacJobService(ISolfacRepository solfacRepository,
            ICrmInvoiceService crmInvoiceService,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions)
        {
            this.solfacRepository = solfacRepository;
            this.crmInvoiceService = crmInvoiceService;
            this.mailSender = mailSender;
            emailConfig = emailConfigOptions.Value;
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

        private List<Email> GetEmailByHitos(List<CrmHito> hitos)
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
                content.AppendLine($"<li>{item.Name} - {item.ProjectName} - {item.ScheduledDate.ToString(DateFormat)}");
            }

            var body = template.Replace("{content}", $"<ul>{content.ToString()}</ul>");

            body = body.Replace("{siteUrl}", emailConfig.SiteUrl);

            return body;
        }
    }
}
