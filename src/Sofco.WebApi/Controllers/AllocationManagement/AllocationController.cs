using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
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
        public IActionResult Post([FromBody] AllocationAsignmentParams parameters)
        {
            var response = allocationService.Add(parameters);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{employeeId}/{startDate}/{endDate}")]
        public IActionResult GetAllocations(int employeeId, DateTime startDate, DateTime endDate)
        {
            var model = allocationService.GetAllocations(employeeId, startDate, endDate);

            return Ok(model.Select(x => new AllocationModel(x)));
        }
    }
}