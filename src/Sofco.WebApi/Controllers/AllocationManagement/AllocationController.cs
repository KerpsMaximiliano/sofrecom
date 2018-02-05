using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.AllocationManagement;

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

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
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

            return Ok(resources.Select(x => new EmployeeViewModel(x)));
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
            var options = new List<SelectListItem>();

            options.Add(new SelectListItem { Value = "999", Text = "<> 100%" });

            var percentages = allocationService.GetAllPercentages();

            options.AddRange(percentages.Select(x => new SelectListItem { Value = x.ToString(CultureInfo.InvariantCulture), Text = $"{x.ToString(CultureInfo.InvariantCulture)}%" }));

            if (percentages.All(x => x != 100))
            {
                options.Add(new SelectListItem { Value = "100", Text = "100%" });
            }

            return Ok(options);
        }
    }
}