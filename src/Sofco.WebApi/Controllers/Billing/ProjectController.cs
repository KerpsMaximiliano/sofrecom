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
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Core.Config;
using Sofco.WebApi.Models.Billing;
using Sofco.WebApi.Config;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ISolfacService _solfacService;
        private readonly CrmConfig _crmConfig;
        private readonly IUserService _userService;

        public ProjectController(ISolfacService solfacService, IOptions<CrmConfig> crmOptions, IUserService userService)
        {
            _solfacService = solfacService;
            _crmConfig = crmOptions.Value;
            _userService = userService;
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

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetById(string projectId)
        {
            try
            {
                var project = await GetProjectById(projectId);

                if (project.Id.Equals("00000000-0000-0000-0000-000000000000"))
                {
                    var response = new Response();
                    response.Messages.Add(new Message(Resources.es.Billing.Project.NotFound, MessageType.Error));

                    return BadRequest(response);
                }

                return Ok(project);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> Get(string serviceId)
        {
            try
            {
                var projects = await GetProjects(serviceId);

                return Ok(projects);
            }
            catch
            {
                return BadRequest();
            }
        }

        private async Task<IList<ProjectCrm>> GetProjects(string serviceId)
        {
            var hasDirectorGroup = this._userService.HasDirectorGroup(this.GetUserMail());

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);

                HttpResponseMessage response;

                if (hasDirectorGroup)
                {
                    response = await client.GetAsync($"/api/project?idService={serviceId}");
                }
                else
                {
                    response = await client.GetAsync($"/api/project?idService={serviceId}&idManager={this.GetUserMail()}");
                }
               
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<IList<ProjectCrm>>(stringResult);

                return projects;
            }
        }

        private async Task<ProjectCrm> GetProjectById(string projectId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);
                var response = await client.GetAsync($"/api/project/{projectId}");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var project = JsonConvert.DeserializeObject<ProjectCrm>(stringResult);

                return project;
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
                            hitoCrm.AmmountBilled = existHito.UnitPrice;
                        }

                        if (!hitoCrm.Status.Equals("Pendiente") && !hitoCrm.Status.Equals("Proyectado"))
                        {
                            hitoCrm.Billed = true;
                        }

                        if(hitoCrm.Status.Equals("A ser facturado"))
                        {
                            hitoCrm.Status = HitoStatus.ToBeBilled.ToString();
                        }
                    }

                    return Ok(hitosCRM);

                }
                catch
                {
                    return BadRequest();
                }
            }
        }
    }
}
