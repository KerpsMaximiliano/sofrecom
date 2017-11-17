using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm;
using Sofco.Model.Enums;

namespace Sofco.Service.Implementations.Billing
{
    public class ProjectService : IProjectService
    {
        private readonly ISolfacService solfacService;

        private readonly CrmConfig crmConfig;

        public ProjectService(ISolfacService solfacService, IOptions<CrmConfig> crmOptions)
        {
            this.solfacService = solfacService;

            crmConfig = crmOptions.Value;
        }

        public IList<CrmProjectHito> GetHitosByProject(string projectId)
        {
            var hitos = solfacService.GetHitosByProject(projectId);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);
                var response = client.GetAsync($"/api/InvoiceMilestone?idProject={projectId}").Result;
                response.EnsureSuccessStatusCode();

                var stringResult = response.Content.ReadAsStringAsync().Result;

                var crmProjectHitos = JsonConvert.DeserializeObject<IList<CrmProjectHito>>(stringResult);

                foreach (var hitoCrm in crmProjectHitos)
                {
                    var existHito = hitos.SingleOrDefault(x => x.ExternalHitoId == hitoCrm.Id);

                    if ((!hitoCrm.Status.Equals("Pendiente") && !hitoCrm.Status.Equals("Proyectado")) || existHito != null)
                    {
                        hitoCrm.Billed = true;
                    }

                    if (hitoCrm.Status.Equals("A ser facturado"))
                    {
                        hitoCrm.Status = HitoStatus.ToBeBilled.ToString();
                    }
                }

                return crmProjectHitos;
            }
        }
    }
}
