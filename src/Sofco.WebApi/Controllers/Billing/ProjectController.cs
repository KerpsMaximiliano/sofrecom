﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Core.Config;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ISolfacService solfacService;
        private readonly CrmConfig crmConfig;
        private readonly IUserService userService;
        private readonly IProjectService projectService;

        public ProjectController(ISolfacService solfacService, IOptions<CrmConfig> crmOptions, IUserService userService, IProjectService projectService)
        {
            this.solfacService = solfacService;
            crmConfig = crmOptions.Value;
            this.userService = userService;
            this.projectService = projectService;
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
                    response.Messages.Add(new Message(Resources.Billing.Project.NotFound, MessageType.Error));

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
                return BadRequest(new List<ProjectCrm>());
            }
        }

        private async Task<IList<ProjectCrm>> GetProjects(string serviceId)
        {
            var hasDirectorGroup = this.userService.HasDirectorGroup(this.GetUserMail());

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

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
                client.BaseAddress = new Uri(crmConfig.Url);
                var response = await client.GetAsync($"/api/project/{projectId}");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var project = JsonConvert.DeserializeObject<ProjectCrm>(stringResult);

                return project;
            }
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
