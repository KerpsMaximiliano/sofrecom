using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.Logger;
using Sofco.Domain.Crm;
using Sofco.Model.Enums;
using Sofco.Model.Helpers;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Framework.CrmServices
{
    public class CrmInvoiceService : ICrmInvoiceService
    {
        private readonly ICrmHttpClient client;

        private readonly CrmConfig crmConfig;

        private readonly ILogMailer<CrmInvoiceService> logger;

        public CrmInvoiceService(ICrmHttpClient client, 
            IOptions<CrmConfig> crmOptions,
            ILogMailer<CrmInvoiceService> logger
            )
        {
            this.client = client;
            crmConfig = crmOptions.Value;
            this.logger = logger;
        }

        public Result<List<CrmHito>> GetHitosToExpire(int daysToExpire)
        {
            var url = $"{crmConfig.Url}/api/InvoiceMilestone/GetMilestoneToExpire?daysToExpire={daysToExpire}&status=1,717620003";

            return client.GetMany<CrmHito>(url);
        }

        public Result<string> CreateHitoBySolfac(Solfac solfac)
        {
            var hito = solfac.Hitos.First();

            var ammount = SolfacHelper.IsCreditNote(solfac) ? -1*hito.Details.Sum(s => s.Total) : hito.Details.Sum(s => s.Total);
            var statusCode = (int)HitoStatus.Pending;
            var startDate = $"{DateTime.UtcNow:O}";
            var name = GetPrefixTitle(solfac) + hito.Description;

            var result = new Result<string>();

            var data =
                $"Ammount={ammount}&StatusCode={statusCode}&StartDate={startDate:O}"
                + $"&Name={name}&MoneyId={hito.CurrencyId}"
                + $"&Month={hito.Month}&ProjectId={hito.ExternalProjectId}"
                + $"&OpportunityId={hito.OpportunityId}&ManagerId={hito.ManagerId}";

            try
            {
                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var clientResult = client.Post<string>($"{crmConfig.Url}/api/InvoiceMilestone", stringContent);

                result.Data = CleanStringResult(clientResult.Data);
            }
            catch (Exception ex)
            {
                logger.LogError(data, ex);
                result.AddError(Resources.Billing.Solfac.ErrorSaveOnHitos);
            }

            return result;
        }

        public Response UpdateHitos(ICollection<Hito> hitos)
        {
            var result = new Response();

            foreach (var item in hitos)
            {
                var sum = item.Details.Sum(x => x.Total);

                if (sum == item.Total) continue;

                var total = $"Ammount={sum}";

                var urlPath = $"{crmConfig.Url}/api/InvoiceMilestone/{item.ExternalHitoId}";

                try
                {
                    var stringContent = new StringContent(total, Encoding.UTF8, "application/x-www-form-urlencoded");

                    client.Put<string>(urlPath, stringContent);
                }
                catch (Exception ex)
                {
                    logger.LogError(total+" - "+urlPath, ex);
                    result.Messages.Add(new Message(Resources.Billing.Solfac.ErrorSaveOnHitos, MessageType.Warning));
                }
            }

            return result;
        }

        public void UpdateHitoStatus(List<Hito> hitos, HitoStatus hitoStatus)
        {
            var statusCode = (int) hitoStatus;

            foreach (var hito in hitos)
            {
                try
                {
                    var stringContent = new StringContent($"StatusCode={statusCode}", Encoding.UTF8, "application/x-www-form-urlencoded");

                    client.Put<string>($"{crmConfig.Url}/api/InvoiceMilestone/{hito.ExternalHitoId}", stringContent);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);
                }
            }
        }

        private string CleanStringResult(string data)
        {
            var result = data.Replace("\"", "");

            return result;
        }

        private string GetPrefixTitle(Solfac solfac)
        {
            if (SolfacHelper.IsCreditNote(solfac))
                return Resources.Billing.Invoice.CrmPrefixCreditNoteTitle;

            if (SolfacHelper.IsDebitNote(solfac))
                return Resources.Billing.Invoice.CrmPrefixDebitNoteTitle;

            return string.Empty;
        }
    }
}
