using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.CrmServices;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Model;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.DAL;
using Sofco.Service.Settings.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class SolfacJobService : ISolfacJobService
    {
        private const string DateFormat = "dd/MM/yyyy";

        private const string Subject = "HITOS sin Solfac";

        private readonly int daysToExpire;

        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        public SolfacJobService(IUnitOfWork unitOfWork,
            ICrmInvoiceService crmInvoiceService,
            IMailBuilder mailBuilder,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions,
            IOptions<JobSetting> setting)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            emailConfig = emailConfigOptions.Value;
            daysToExpire = setting.Value.SolfacJob.DaysToExpire;
        }

        public void SendHitosNotifications()
        {
            var hitos = GetHitosWithoutSolfac();

            var emails = GetEmailByHitos(hitos);

            mailSender.Send(emails);
        }

        private List<CrmHito> GetHitosWithoutSolfac()
        {
            var crmHitosResult = crmInvoiceService.GetHitosToExpire(daysToExpire);

            var crmHitosList = crmHitosResult.Data.ToList();

            var crmHitosIdList = crmHitosList.Select(s => s.Id).ToList();

            var hitos = unitOfWork.SolfacRepository.GetHitosByExternalIds(crmHitosIdList);

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

                var mailContent = GetContent(item.Value);

                result.Add(mailBuilder.GetEmail(
                    MailType.HitosWithoutSolfac, item.Key, Subject, mailContent));
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

        private Dictionary<string, string> GetContent(List<CrmHito> hitos)
        {
            var content = new StringBuilder();

            foreach (var item in hitos)
            {
                var link = $"{emailConfig.SiteUrl}/billing/" +
                           $"customers/{item.CustomerId}" +
                           $"/services/{item.ServiceId}" +
                           $"/projects/{item.ProjectId}";

                content.AppendLine($"<li><a href='{link}'>{item.Name} - {item.ProjectName} - {item.ScheduledDate.ToString(DateFormat)}</a>");
            }

            return new Dictionary<string, string>
            {
                {MailContentKey.Content, $"<ul>{content}</ul>"}
            };
        }
    }
}
