using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Admin;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/modules")]
    [Authorize]
    public class ModuleController : Controller
    {
        private readonly IModuleService moduleService;
        private readonly IFunctionalityService functionalityService;

        public ModuleController(IModuleService moduleService, IFunctionalityService functionalityService)
        {
            this.functionalityService = functionalityService;
            this.moduleService = moduleService;
        }

        // GET: api/module
        [HttpGet]
        public IActionResult Get()
        {
            var modules = moduleService.GetAllReadOnly(false);
            var model = new List<ModuleModel>();

            foreach (var module in modules)
                model.Add(new ModuleModel(module));

            return Ok(model.OrderBy(x => x.Description));
        }

        // GET: api/module/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var modules = moduleService.GetAllReadOnly(true);
            var model = new List<SelectListItem>();

            foreach (var module in modules)
                model.Add(new SelectListItem { Value = module.Id.ToString(), Text = module.Description });

            return Ok(model);
        }

        [HttpGet]
        [Route("modulesAndFunctionalities")]
        public IActionResult GetOptionsWithFunctionalities()
        {
            var modules = moduleService.GetAllWithFunctionalitiesReadOnly();
            var model = new List<ModuleModel>();

            foreach (var module in modules)
            {
                var moduleToAdd = new ModuleModel(module);

                foreach (var funct in module.Functionalities)
                {
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
            var response = moduleService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

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
            var response = moduleService.Active(id, active);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody]ModuleModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var moduleResponse = moduleService.GetById(model.Id);

            if (moduleResponse.HasErrors())
                return BadRequest(moduleResponse);

            model.ApplyTo(moduleResponse.Data);

            var response = moduleService.Update(moduleResponse.Data);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}/functionalities")]
        public IActionResult GetFunctionalities(int id)
        {
            var response = functionalityService.GetFunctionalitiesByModule(id);

            var model = new List<FunctionalityModel>();

            foreach (var functionality in response)
                model.Add(new FunctionalityModel(functionality));

            return Ok(model);
        }
    }
}
