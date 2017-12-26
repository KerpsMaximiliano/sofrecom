using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.DTO;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ISolfacService solfacService;
        private readonly IProjectService projectService;

        public ProjectController(ISolfacService solfacService, IProjectService projectService)
        {
            this.solfacService = solfacService;
            this.projectService = projectService;
        }

        [HttpGet("{serviceId}/options")]
        public IActionResult GetOptions(string serviceId)
        {
            try
            {
                IList<CrmProject> customers = this.projectService.GetProjects(serviceId, this.GetUserMail(), this.GetUserName());
                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre });

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{projectId}")]
        public IActionResult GetById(string projectId)
        {
            try
            {
                var response = this.projectService.GetProjectById(projectId);

                if (response.HasErrors()) return BadRequest(response);

                return Ok(response.Data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("service/{serviceId}")]
        public IActionResult Get(string serviceId)
        {
            try
            {
                var projects = this.projectService.GetProjects(serviceId, this.GetUserMail(), this.GetUserName());

                return Ok(projects);
            }
            catch
            {
                return BadRequest(new List<ProjectCrm>());
            }
        }

        [HttpGet]
        [Route("{projectId}/hitos/reload/{reload}")]
        public IActionResult GetHitos(string projectId, bool reload)
        {
            try
            {
                return Ok(projectService.GetHitosByProject(projectId, reload));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("hitos/new")]
        public async Task<IActionResult> SplitHito([FromBody] HitoSplittedParams hito)
        {
            var response = await solfacService.SplitHito(hito);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }
    }
}
