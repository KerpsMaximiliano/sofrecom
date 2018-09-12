using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/userApprovers")]
    [Authorize]
    public class UserApproversController : Controller
    {
        private readonly IUserApproverService service;

        private readonly IUserApproverEmployeeService approversEmployeeService;

        public UserApproversController(IUserApproverService service, IUserApproverEmployeeService approversEmployeeService)
        {
            this.service = service;
            this.approversEmployeeService = approversEmployeeService;
        }

        [HttpGet("{type}")]
        public IActionResult Get(UserApproverType type, [FromUri] UserApproverQuery query)
        {
            var response = service.GetApprovers(query, type);

            return this.CreateResponse(response);
        }

        [HttpGet("{type}/employees")]
        public IActionResult GetEmployees(UserApproverType type, [FromUri] UserApproverQuery query)
        {
            var response = approversEmployeeService.Get(query, type);

            return this.CreateResponse(response);
        }

        [HttpPost("{type}")]
        public IActionResult Post(UserApproverType type, [FromBody]List<UserApproverModel> model)
        {
            var response = service.Save(model, type);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = service.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpPost("clean")]
        public IActionResult Delete([FromBody] List<int> ids)
        {
            var response = new Response();

            foreach (var id in ids)
            {
                response = service.Delete(id);

                if (response.HasErrors())
                    break;
            }

            return this.CreateResponse(response);
        }
    }
}
