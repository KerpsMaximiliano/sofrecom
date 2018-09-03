using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/workTimeApprovals")]
    [Authorize]
    public class WorkTimeApprovalsController : Controller
    {
        private readonly IWorkTimeApprovalService service;

        public WorkTimeApprovalsController(IWorkTimeApprovalService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = service.GetAll();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody]List<WorkTimeApprovalModel> workTimeApprovals)
        {
            var response = service.Save(workTimeApprovals);

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

                if(response.HasErrors()) break;
            }

            return this.CreateResponse(response);
        }
    }
}
