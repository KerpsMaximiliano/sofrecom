using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class HitoService : IHitoService
    {
        private readonly CrmConfig crmConfig;

        public HitoService(IOptions<CrmConfig> crmOptions)
        {
            this.crmConfig = crmOptions.Value;
        }

        public Response Close(string id)
        {
            var response = new Response();

            var closeStatusCode = crmConfig.CloseStatusCode;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                try
                {
                    var stringContent = new StringContent("StatusCode="+ closeStatusCode, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var result = client.PutAsync($"/api/InvoiceMilestone/{id}", stringContent).Result;

                    result.EnsureSuccessStatusCode();

                    response.Messages.Add(new Message(Resources.Billing.Solfac.CloseHito, MessageType.Success));
                }
                catch (Exception)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.ErrorSaveOnHitos, MessageType.Error));
                    return response;
                }
            }

            return response;
        }
    }
}
