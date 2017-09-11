using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/project")]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ISolfacService _solfacService;
        private readonly CrmConfig _crmConfig;

        public ProjectController(ISolfacService solfacService, IOptions<CrmConfig> crmOptions)
        {
            _solfacService = solfacService;
            _crmConfig = crmOptions.Value;
        }

        [HttpGet("{serviceId}/options")]
        public async Task<IActionResult> GetOptions(string serviceId)
        {
            try
            {
                IList<ProjectCrm> customers = await GetProjects(serviceId);
                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre });

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> Get(string serviceId)
        {
            try
            {
                var projects = await GetProjects(serviceId);

                return Ok(projects);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        private async Task<IList<ProjectCrm>> GetProjects(string serviceId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);
                var response = await client.GetAsync($"/api/project?idService={serviceId}");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<IList<ProjectCrm>>(stringResult);

                return projects;
            }
        }

        [HttpGet]
        [Route("{projectId}/hitos")]
        public async Task<IActionResult> GetHitos(string projectId)
        {
            var hitos = _solfacService.GetHitosByProject(projectId);

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_crmConfig.Url);
                    var response = await client.GetAsync($"/api/InvoiceMilestone?idProject={projectId}");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var hitosCRM = JsonConvert.DeserializeObject<IList<HitoCrm>>(stringResult);

                    foreach (var hitoCrm in hitosCRM)
                    {
                        var existHito = hitos.SingleOrDefault(x => x.ExternalHitoId == hitoCrm.Id);

                        if (existHito != null)
                        {
                            hitoCrm.Billed = true;
                            hitoCrm.AmmountBilled = existHito.Total;
                        }
                        else
                        {
                            hitoCrm.Billed = false;
                        }
                    }

                    return Ok(hitosCRM);

                }
                catch (Exception e)
                {
                    return BadRequest();
                }
            }
        }
    }
}
