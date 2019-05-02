using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService projectService;

        public ProjectController(IProjectService projectService)
        {
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

        [HttpGet]
        [Route("{projectId}/purchaseOrders")]
        public IActionResult GetPurchaseOrders(string projectId)
        {
            try
            {
                return Ok(projectService.GetPurchaseOrders(projectId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Put()
        {
            var response = projectService.Update();

            return this.CreateResponse(response);
        }
    }
}
