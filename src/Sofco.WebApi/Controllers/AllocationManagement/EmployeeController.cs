﻿using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Models.AllocationManagement;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employees")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeServ)
        {
            employeeService = employeeServ;
        }

        [HttpGet("options")]
        public IActionResult Get()
        {
            var model = employeeService.GetAll().Select(x => new EmployeeOptionModel(x));
            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = employeeService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(new EmployeeOptionModel(response.Data));
        }
    }
}
