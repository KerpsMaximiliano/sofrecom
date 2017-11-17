﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Domain.Crm;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Framework.CrmServices
{
    public class CrmInvoiceService : ICrmInvoiceService
    {
        private readonly ICrmHttpClient client;

        private readonly CrmConfig crmConfig;

        public CrmInvoiceService(ICrmHttpClient client, IOptions<CrmConfig> crmOptions)
        {
            this.client = client;
            crmConfig = crmOptions.Value;
        }

        public Result<List<CrmHito>> GetHitosToExpire(int daysToExpire)
        {
            var url = $"{crmConfig.Url}/api/InvoiceMilestone/GetMilestoneToExpire?daysToExpire={daysToExpire}&status=1,717620003";

            return client.GetMany<CrmHito>(url);
        }

        public Result<string> CreateHitoBySolfac(Solfac solfac)
        {
            var hito = solfac.Hitos.First();

            var ammount = hito.Details.Sum(s => s.Total);
            var statusCode = HitoStatus.Pending;
            var startDate = DateTime.Now;
            var name = hito.Description;
            var moneyId = hito.CurrencyId; //TODO: add column to database - frontend: project.currencyId
            var opportunityId = hito.OpportunityId; //TODO: add column to database - frontend: project.OpportunityId
            var managerId = hito.ManagerId; //TODO: add column to database - frontend: project.ManagerId

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

                result.Data = clientResult.Data;
            }
            catch (Exception)
            {
                result.AddError(Resources.es.Billing.Solfac.ErrorSaveOnHitos);
            }

            return result;
        }
    }
}
