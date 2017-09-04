using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/module")]
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly IModuleService _moduleService;
        private readonly IFunctionalityService _functionalityService;

        public ModuleController(IModuleService moduleService, IFunctionalityService functionalityService)
        {
            _functionalityService = functionalityService;
            _moduleService = moduleService;
        }

        // GET: api/module
        [HttpGet]
        public IActionResult Get()
        {
            var modules = _moduleService.GetAllReadOnly(false);
            var model = new List<ModuleModel>();

            foreach (var module in modules)
                model.Add(new ModuleModel(module));

            return Ok(model);
        }

        // GET: api/module/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var modules = _moduleService.GetAllReadOnly(true);
            var model = new List<SelectListItem>();

            foreach (var module in modules)
                model.Add(new SelectListItem { Value = module.Id.ToString(), Text = module.Description });

            return Ok(model);
        }

        [HttpGet]
        [Route("modulesAndFunctionalities")]
        public IActionResult GetOptionsWithFunctionalities()
        {
            var modules = _moduleService.GetAllWithFunctionalitiesReadOnly();
            var model = new List<ModuleModel>();

            foreach (var module in modules)
            {
                var moduleToAdd = new ModuleModel(module);

                foreach (var funct in module.Functionalities) {
                    moduleToAdd.Functionalities.Add(new FunctionalityModel(funct));
                }

                model.Add(moduleToAdd);
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

            foreach (var functionality in response.Data.Functionalities)
            {
                model.Functionalities.Add(new FunctionalityModel(functionality));
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

        [HttpGet("{id}/functionalities")]
        public IActionResult GetFunctionalities(int id)
        {
            var response = _functionalityService.GetFunctionalitiesByModule(id);

            var model = new List<FunctionalityModel>();

            foreach (var functionality in response)
                model.Add(new FunctionalityModel(functionality));

            return Ok(model);
        }
    }
}
