using System;
using Microsoft.Extensions.Options;
using Sofco.Common.Domains;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.Logger;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Framework.CrmServices
{
    public class CrmService : ICrmService
    {
        private readonly ICrmHttpClient client;

        private readonly CrmConfig crmConfig;

        private readonly ILogMailer<CrmService> logger;

        public CrmService(ICrmHttpClient client, IOptions<CrmConfig> crmOptions, ILogMailer<CrmService> logger)
        {
            this.client = client;
            crmConfig = crmOptions.Value;
            this.logger = logger;
        }

        public Result<string> DesactivateService(Guid id)
        {
            return ProcessActivate(id, false);
        }

        public Result<string> ActivateService(Guid id)
        {
            return ProcessActivate(id);
        }

        private Result<string> ProcessActivate(Guid id, bool activate = true)
        {
            var result = new Result<string>();

            var url = $"{crmConfig.Url}/api/service/{id}/" + (activate ? "activate" : "desactivate");

            try
            {
                result.Data = client.Get<string>(url).Data;
            }
            catch (Exception ex)
            {
                logger.LogError(url, ex);

                result.AddError(ex.Message);
            }

            return result;
        }
    }
}
