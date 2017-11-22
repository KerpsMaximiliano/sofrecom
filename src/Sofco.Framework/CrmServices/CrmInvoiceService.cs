using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Domain.Crm;
using Sofco.Model.Enums;
using Sofco.Model.Helpers;
using Sofco.Model.Models.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Framework.CrmServices
{
    public class CrmInvoiceService : ICrmInvoiceService
    {
        private readonly ICrmHttpClient client;

        private readonly CrmConfig crmConfig;

        private readonly ILoggerWrapper<CrmInvoiceService> logger;

        public CrmInvoiceService(ICrmHttpClient client, 
            IOptions<CrmConfig> crmOptions,
            ILoggerWrapper<CrmInvoiceService> logger
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
            var startDate = DateTime.Now;

            var result = new Result<string>();
            try
            {
                var data =
                    $"Ammount={ammount}&StatusCode={statusCode}&StartDate={startDate:O}"
                    + $"&Name={hito.Description}&MoneyId={hito.CurrencyId}"
                    + $"&Month={hito.Month}&ProjectId={hito.ExternalProjectId}"
                    + $"&OpportunityId={hito.OpportunityId}&ManagerId={hito.ManagerId}";

                var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var clientResult = client.Post<string>($"{crmConfig.Url}/api/InvoiceMilestone", stringContent);

                result.Data = CleanStringResult(clientResult.Data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                result.AddError(Resources.es.Billing.Solfac.ErrorSaveOnHitos);
            }

            return result;
        }

        private string CleanStringResult(string data)
        {
            var result = data.Replace("\"", "");

            return result;
        }
    }
}
