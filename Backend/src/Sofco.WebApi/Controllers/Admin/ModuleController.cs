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
            var model = new List<Option<int>>();

            foreach (var module in modules)
                model.Add(new Option<int>(module.Id, module.Description));

            return Ok(model);
        }

        // GET api/module/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _moduleService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new ModuleModel(response.Data);

            foreach (var moduleFunctionality in response.Data.ModuleFunctionality)
            {
                if(moduleFunctionality.Functionality.Active)
                    model.Functionalities.Add(new FunctionalityModel(moduleFunctionality.Functionality));
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

        [HttpPost]
        [Route("{moduleId}/functionalities")]
        public IActionResult ChangeFunctionalities(int moduleId, [FromBody]List<int> model)
        {
            var response = _moduleService.ChangeFunctionalities(moduleId, model);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{moduleId}/functionality/{functionalityId}")]
        public IActionResult AddFunctionality(int moduleId, int functionalityId)
        {
            var response = _moduleService.AddFunctionality(moduleId, functionalityId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{moduleId}/functionality/{functionalityId}")]
        public IActionResult DeleteFunctionality(int moduleId, int functionalityId)
        {
            var response = _moduleService.DeleteFunctionality(moduleId, functionalityId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
