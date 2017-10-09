using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Domain.Crm;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Framework.CrmServices
{
    public class CrmInvoiceService : ICrmInvoiceService
    {
        private readonly IBaseHttpClient<CrmHito> client;

        private readonly CrmConfig crmConfig;

        public CrmInvoiceService(IBaseHttpClient<CrmHito> client, IOptions<CrmConfig> crmOptions)
        {
            this.client = client;
            crmConfig = crmOptions.Value;
        }

        public Result<List<CrmHito>> Get(int daysToExpire)
        {
            var url = $"{crmConfig.Url}/api/InvoiceMilestone/GetMilestoneToExpire?daysToExpire={daysToExpire}&status=1,717620003";

            return client.GetMany(url);
        }
    }
}
