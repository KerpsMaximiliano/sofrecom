using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;
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
        public IActionResult Post([FromBody]List<WorkTimeApproval> workTimeApprovals)
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
    }
}
