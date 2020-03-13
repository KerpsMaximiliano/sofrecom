using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.DTO;
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
            var model = allocationService.GetAllocationsBetweenDays(employeeId, startDate.Date, endDate.Date, new List<int>());

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
            var list = new List<EmployeeModel>();

            foreach (var resource in resources)
            {
                if (resource.Allocations != null && resource.Allocations.Any())
                {
                    var allocation = resource.Allocations.FirstOrDefault(x => x.AnalyticId == analyticId && x.StartDate.Year == DateTime.Now.Year && x.StartDate.Month == DateTime.Now.Month);

                    if (allocation != null && allocation.Percentage > 0)
                    {
                        var item = new EmployeeModel(resource);

                        item.PercentageAllocation = allocation.Percentage;

                        list.Add(item);
                    }
                }
            }

            return Ok(list);
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