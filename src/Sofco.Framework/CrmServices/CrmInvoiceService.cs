using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.Logger;
using Sofco.Domain.Crm;
using Sofco.Model.DTO;
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
            ILogMailer<CrmInvoiceService> logger)
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

            hito.Total = hito.Details.Sum(s => s.Total);

            var ammount = hito.Total.ToString("0.##", new CultureInfo("en-US"));
            var ammountText = SolfacHelper.IsCreditNote(solfac) ? "-"+ammount : ammount;
            var statusCode = (int)HitoStatus.Pending;
            var startDate = DateTime.UtcNow;
            var name = hito.Description = GetPrefixTitle(solfac) + hito.Description;

            var result = new Result<string>();

            var data =
                $"Ammount={ammountText}&StatusCode={statusCode}&StartDate={startDate:O}"
                + $"&Name={name}&MoneyId={hito.CurrencyId}"
                + $"&Month={hito.Month}&ProjectId={hito.ExternalProjectId}"
                + $"&OpportunityId={hito.OpportunityId}&ManagerId={hito.ManagerId}";

            try
            {
                var content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var clientResult = client.Post<string>($"{crmConfig.Url}/api/InvoiceMilestone", content);

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

                var content = $"Ammount={sum}";

                ProcessUpdateHitos(item.ExternalHitoId, content);

            }

            return result;
        }

        public void UpdateHitoStatus(List<Hito> hitos, HitoStatus hitoStatus)
        {
            UpdateHitoStatus(hitos.Select(s => s.ExternalHitoId).ToList(), hitoStatus);
        }

        public void UpdateHitoStatus(List<string> hitoIds, HitoStatus hitoStatus)
        {
            var statusCode = (int)hitoStatus;

            foreach (var hitoId in hitoIds)
            {
                ProcessUpdateHitos(hitoId, $"StatusCode={statusCode}");
            }
        }

        public void UpdateHitoInvoice(IList<Hito> hitos, SolfacStatusParams parameters)
        {
            foreach (var hito in hitos)
            {
                ProcessUpdateHitos(hito.ExternalHitoId, $"InvoicingDate={parameters.InvoiceDate.GetValueOrDefault():O}&InvoicingNumber={parameters.InvoiceCode}");
            }
        }

        public void UpdateHitosStatus(List<string> hitoIds, HitoStatus hitoStatus)
        {
            var statusCode = (int)hitoStatus;

            foreach (var hitoId in hitoIds)
            {
                ProcessUpdateHitos(hitoId, $"StatusCode={statusCode}");
            }
        }

        public void UpdateHitosStatusAndPurchaseOrder(List<string> hitoIds, HitoStatus hitoStatus, string purchaseOrderNumber)
        {
            var statusCode = (int)hitoStatus;

            foreach (var hitoId in hitoIds)
            {
                ProcessUpdateHitos(hitoId, $"StatusCode={statusCode}&PurchaseOrder={purchaseOrderNumber}");
            }
        }

        public void UpdateHitosStatusAndInvoiceDateAndNumber(List<string> hitoIds, HitoStatus hitoStatus, DateTime invoicingDate,
            string invoiceCode)
        {
            var statusCode = (int)hitoStatus;

            foreach (var hitoId in hitoIds)
            {
                ProcessUpdateHitos(hitoId, $"StatusCode={statusCode}&InvoicingDate={invoicingDate:O}&InvoicingNumber={invoiceCode}");
            }
        }

        public void UpdateHitosStatusAndBillingDate(List<string> hitoIds, HitoStatus hitoStatus, DateTime billingDate)
        {
            var statusCode = (int)hitoStatus;

            foreach (var hitoId in hitoIds)
            {
                ProcessUpdateHitos(hitoId, $"StatusCode={statusCode}&BillingDate={billingDate:O}");
            }
        }

        private void ProcessUpdateHitos(string hitoId, string content)
        {
            var urlPath = $"{crmConfig.Url}/api/InvoiceMilestone/{hitoId}";

            try
            {
                var stringContent = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");

                client.Put<string>(urlPath, stringContent);
            }
            catch (Exception ex)
            {
                logger.LogError(urlPath +" - "+ content, ex);
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
