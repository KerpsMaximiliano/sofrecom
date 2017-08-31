using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/project")]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ISolfacService _solfacService;

        public ProjectController(ISolfacService solfacService)
        {
            _solfacService = solfacService;
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> Get(string serviceId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://sofrelab-iis1.cloudapp.net:4090");
                    var response = await client.GetAsync($"/api/project?idService={serviceId}");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var projects = JsonConvert.DeserializeObject<IList<ProjectCrm>>(stringResult);

                    return Ok(projects);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
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
                    client.BaseAddress = new Uri("http://sofrelab-iis1.cloudapp.net:4090");
                    var response = await client.GetAsync($"/api/InvoiceMilestone?idProject={projectId}");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var hitosCRM = JsonConvert.DeserializeObject<IList<HitoCrm>>(stringResult);

                    foreach (var hitoCrm in hitosCRM)
                    {
                        hitoCrm.Billed = hitos.Any(x => x.ExternalHitoId == hitoCrm.Id);
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
