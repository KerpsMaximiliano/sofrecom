using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/allocations")]
    [Authorize]
    public class AllocationController : Controller
    {
        private readonly IAllocationService allocationService;

        public AllocationController(IAllocationService allocationServ)
        {
            allocationService = allocationServ;
        }

        [HttpPost]
        public IActionResult Post([FromBody] AllocationDto allocation)
        {
            var response = allocationService.Add(allocation);

            return this.CreateResponse(response);
        }

        [HttpPost("massive")]
        public IActionResult Post([FromBody] AllocationMassiveAddModel model)
        {
            var response = allocationService.AddMassive(model);

            if (response.HasErrors())
                return BadRequest(response);

            if (response.Data == null)
                return Ok();

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("analytics/{employeeId}/{startDate}/{endDate}")]
        public IActionResult GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            var model = allocationService.GetAllocationsBetweenDays(employeeId, startDate.Date, endDate.Date);

            return Ok(model);
        }

        [HttpGet("service/{serviceId}")]
        public IActionResult GetByService(string serviceId)
        {
            var resources = allocationService.GetByService(serviceId);

            return Ok(resources.Select(x => new EmployeeModel(x)));
        }

        [HttpGet("analytic/{analyticId}")]
        public IActionResult GetByAnalytic(int analyticId)
        {
            var resources = allocationService.GetByEmployeesByAnalytic(analyticId);

            return Ok(resources.Select(x => new EmployeeModel(x)));
        }

        [HttpPost("report")]
        public IActionResult Report([FromBody] AllocationReportParams parameters)
        {
            var response = allocationService.CreateReport(parameters);

            return this.CreateResponse(response);
        }

        [HttpGet("percentages")]
        public IActionResult GetAllPercentages()
        {
            var percentages = allocationService.GetAllPercentages();

            return Ok(percentages);
        }
    }
}