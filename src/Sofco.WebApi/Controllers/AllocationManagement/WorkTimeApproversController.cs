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
    [Route("api/workTimeApprovers")]
    [Authorize]
    public class WorkTimeApproversController : Controller
    {
        private readonly IUserApproverService service;

        public WorkTimeApproversController(IUserApproverService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get([FromUri] UserApproverQuery query)
        {
            var response = service.GetApprovers(query, UserApproverType.WorkTime);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody]List<UserApproverModel> model)
        {
            var response = service.Save(model, UserApproverType.WorkTime);

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
