using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.Billing;
using Sofco.Model.DTO;
using Sofco.WebApi.Extensions;

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
                var respone = projectService.GetProjects(serviceId, this.GetUserMail(), this.GetUserName());

                if (respone.HasErrors()) return BadRequest(respone);

                var customers = respone.Data;

                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre }).OrderBy(x => x.Text);

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
            var response = projectService.GetProjects(serviceId, this.GetUserMail(), this.GetUserName());

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response.Data);
        }

        [HttpGet]
        [Route("{projectId}/hitos")]
        public IActionResult GetHitos(string projectId)
        {
            try
            {
                return Ok(projectService.GetHitosByProject(projectId));
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
