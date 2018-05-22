using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var respone = projectService.GetProjectsOptions(serviceId);

            return this.CreateResponse(respone);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetById(string projectId)
        {
            try
            {
                var response = projectService.GetProjectById(projectId);

                if (response.HasErrors())
                    return BadRequest(response);

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
            var response = projectService.GetProjects(serviceId);

            return this.CreateResponse(response);
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
    }
}
