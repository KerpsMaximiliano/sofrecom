using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;

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
    }
}