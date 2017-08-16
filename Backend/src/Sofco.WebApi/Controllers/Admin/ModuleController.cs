using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/module")]
    public class ModuleController : Controller
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        // GET: api/module
        [HttpGet]
        public IActionResult Get()
        {
            var modules = _moduleService.GetAllReadOnly();
            var model = new List<ModuleModel>();

            foreach (var module in modules)
            {
                model.Add(new ModuleModel(module));
            }

            return Ok(model);
        }

        // GET: api/module/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var modules = _moduleService.GetAllReadOnly();
            var model = new List<Option<int>>();

            foreach (var module in modules)
            {
                model.Add(new Option<int>(module.Id, module.Description));
            }

            return Ok(model);
        }

        // GET: api/module
        [HttpGet]
        [Route("full")]
        public IActionResult GetFull()
        {
            var modules = _moduleService.GetAllFullReadOnly();
            var model = new List<ModuleModel>();

            foreach (var module in modules)
            {
                var moduleModel = new ModuleModel(module);

                foreach (var roleModuleFunctionality in module.RoleModuleFunctionality)
                {
                    moduleModel.Functionalities.Add(new FunctionalityModel(roleModuleFunctionality.Functionality));
                }

                model.Add(moduleModel);
            }

            return Ok(model);
        }

        // GET api/module/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _moduleService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new ModuleModel(response.Data);

            foreach (var roleModuleFunctionality in response.Data.RoleModuleFunctionality)
            {
                model.Functionalities.Add(new FunctionalityModel(roleModuleFunctionality.Functionality));
            }

            return Ok(model);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = _moduleService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody]ModuleModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var moduleResponse = _moduleService.GetById(model.Id);

            if (moduleResponse.HasErrors()) return BadRequest(moduleResponse);

            model.ApplyTo(moduleResponse.Data);

            var response = _moduleService.Update(moduleResponse.Data);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
