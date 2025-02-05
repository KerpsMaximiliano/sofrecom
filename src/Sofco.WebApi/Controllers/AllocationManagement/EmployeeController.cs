﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employees")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly IUserService userService;
        private readonly IAllocationService allocationService;

        public EmployeeController(IEmployeeService employeeService, IUserService userService, IAllocationService allocationService)
        {
            this.employeeService = employeeService;
            this.userService = userService;
            this.allocationService = allocationService;
        }

        [HttpGet]
        public IActionResult Get()
      {
            var model = employeeService.GetAll().Select(x => new EmployeeModel(x));

            return Ok(model.OrderBy(x => x.Name));
        }

        [HttpGet("everyone")]
        public IActionResult GetEveryone()
        {
            var model = employeeService.GetEveryone();

            return Ok(model.OrderBy(x => x.Name));
        }

        [HttpGet("worktimeReport")]
        public IActionResult GetAllForWorkTimeReport()
        {
            var options = employeeService.GetAllForWorkTimeReport().OrderBy(x => x.Name).Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber} - {x.Name}" });

            return Ok(options);
        }

        [HttpPost("search/unemployees")]
        public IActionResult GetUnemployees([FromBody] UnemployeeSearchParameters parameters)
        {
            var model = employeeService.GetUnemployees(parameters);
             
            return Ok(model);
        }

        [HttpPost("search/updown")]
        public IActionResult GetUpdownReport([FromBody] ReportUpdownParameters parameters)
        {
            var response = employeeService.GetUpdownReport(parameters);

            return this.CreateResponse(response);
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = new List<Option>();

            options.AddRange(employeeService.GetAll().OrderBy(x => x.Name).Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber} - {x.Name}" }));

            return Ok(options);
        }

        [HttpGet("options/full")]
        public IActionResult GetOptionsFull()
        {
            var options = new List<Option>();

            options.AddRange(employeeService.GetAllForWorkTimeReport().OrderBy(x => x.Name).Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber} - {x.Name}" }));

            return Ok(options);
        }

        [HttpGet("listItems")]
        public IActionResult GetListItems()
        {
            var model = new List<EmployeeSelectListItem>();

            var employees = employeeService.GetAll().OrderBy(x => x.Name);

            foreach (var employee in employees)
            {
                var user = userService.GetUserInfo(employee.Id);
                model.Add(new EmployeeSelectListItem { Id = employee.Id, Text = employee.Name.ToUpper(), employeeNumber = employee.EmployeeNumber, UserId = user.Data?.Id });
            }

            return Ok(model);
        }

        [HttpGet("{id}/profile")]
        public IActionResult GetProfile(int id)
        {
            var response = employeeService.GetProfile(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = employeeService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/sectorName")]
        public IActionResult GetSectorName(int id)
        {
            var hoy00 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var hoy23 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            hoy23 = hoy23.AddMonths(1);
            hoy23 = hoy23.AddDays(-(hoy23.Day + 1));
            var response = allocationService.GetSectorByEmployee(id, hoy00, hoy23);
            return this.CreateResponse(response);
        }

        [HttpGet("{id}/info")]
        public IActionResult GetInfo(int id)
        {
            var response = userService.GetUserInfo(id);

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] EmployeeSearchParams parameters)
        {
            var searchResponse = employeeService.Search(parameters);

            var response = 
                new Response<IEnumerable<EmployeeModel>>
                {
                    Data = searchResponse.Data.Select(x => new EmployeeModel(x)),
                    Messages = searchResponse.Messages
                };

            return this.CreateResponse(response);
        }

        [HttpPost("sendUnsubscribeNotification")]
        public IActionResult SendUnsubscribeNotification([FromBody] EmployeeEndNotificationModel model)
        {
            var response = employeeService.SendUnsubscribeNotification(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/finalizeExtraHolidays")]
        public IActionResult FinalizeExtraHolidays(int id)
        {
            var response = employeeService.FinalizeExtraHolidays(id);

            return this.CreateResponse(response);
        }

        [HttpPut("categories")]
        public IActionResult AddCategories([FromBody] EmployeeAddCategoriesParams parameters)
        {
            var response = employeeService.AddCategories(parameters);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/analytics")]
        public IActionResult GetAnalytics(int id)
        {
            return Ok(employeeService.GetAnalytics(id));
        }

        [HttpGet("{id}/categories")]
        public IActionResult GetCategories(int id)
        {
            var response = employeeService.GetCategories(id);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] EmployeeBusinessHoursParams model)
        {
            var response = employeeService.Update(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet("currentCategories")]
        public IActionResult GetCurrentCategories()
        {
            var response = employeeService.GetCurrentCategories();

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/pendingWorkingHours")]
        public IActionResult PendingWorkingHours(int id)
        {
            var response = employeeService.GetPendingWorkingHours(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/advancements")]
        public IActionResult GetAdvancements(int id)
        {
            var response = employeeService.GetAdvancements(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/refunds")]
        public IActionResult GetRefunds(int id)
        {
            var response = employeeService.GetRefunds(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/currentAccount")]
        public IActionResult GetCurrentAccount(int id)
        {
            var response = employeeService.GetCurrentAccount(id);

            return this.CreateResponse(response);
        }

        [HttpPost("external")]
        public IActionResult AddExternal([FromBody] AddExternalModel model)
        {
            var response = employeeService.AddExternal(model);

            return this.CreateResponse(response);
        }

        [HttpGet("currentManager/options")]
        public IActionResult GetOptionByCurrentManager()
        {
            var response = employeeService.GetEmployeeOptionByCurrentManager();

            return this.CreateResponse(response);
        }

        [HttpGet("{mail}/infoByMail")]
        public IActionResult GetByMail(string mail)
        {
            var model = employeeService.GetByMail(mail);

            return Ok(model);
        }

        [HttpGet("report")]
        public IActionResult Report()
        {
            var response = employeeService.GetReport();

            if (response.HasErrors())
                return BadRequest(response);

            if (response.Data == null)
                return Ok(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("report/short")]
        public IActionResult ShortReport()
        {
            var response = employeeService.GetShortReport();

            if (response.HasErrors())
                return BadRequest(response);

            if (response.Data == null)
                return Ok(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpPut("updateAssingComment")]
        public IActionResult UpdateAssingComment([FromBody] UpdateAssingCommentModel model)
        {
            var response = employeeService.UpdateAssingComment(model);

            return this.CreateResponse(response);
        }
    }
}
