using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/functionality")]
    public class FunctionalityController : Controller
    {
        private readonly IFunctionalityService _functionalityService;

        public FunctionalityController(IFunctionalityService functionalityService)
        {
            _functionalityService = functionalityService;
        }

        // GET: api/functionality
        [HttpGet]
        public IActionResult Get()
        {
            var functionalities = _functionalityService.GetAllReadOnly();
            var model = new List<FunctionalityModel>();

            foreach (var functionality in functionalities)
            {
                model.Add(new FunctionalityModel(functionality));
            }

            return Ok(model);
        }

        // GET: api/functionality/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var functionalities = _functionalityService.GetAllReadOnly();
            var model = new List<Option<int>>();

            foreach (var functionality in functionalities)
            {
                model.Add(new Option<int>(functionality.Id, functionality.Description));
            }

            return Ok(model);
        }

        // GET: api/functionality
        [HttpGet]
        [Route("full")]
        public IActionResult GetFull()
        {
            var functionalities = _functionalityService.GetAllFullReadOnly();
            var model = new List<FunctionalityModel>();

            foreach (var functionality in functionalities)
            {
                var functionalityModel = new FunctionalityModel(functionality);

                foreach (var roleFunctionality in functionality.RoleModuleFunctionality)
                {
                    functionalityModel.Roles.Add(new RoleModel(roleFunctionality.Role));
                }

                model.Add(functionalityModel);
            }

            return Ok(model);
        }

        // GET api/functionality/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _functionalityService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new FunctionalityModel(response.Data);

            foreach (var roleFunctionality in response.Data.RoleModuleFunctionality)
            {
                model.Roles.Add(new RoleModel(roleFunctionality.Role));
            }

             return Ok(model);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = _functionalityService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpGet]
        [Route("{moduleId}/{roleId}")]
        public IActionResult GetFunctionalitiesByModuleAndRole(int moduleId, int roleId)
        {
            var functionalities = _functionalityService.GetFunctionalitiesByModuleAndRole(moduleId, roleId);

            var model = new List<Option<int>>();

            foreach (var functionality in functionalities)
            {
                model.Add(new Option<int>(functionality.Id, functionality.Description));
            }

            return Ok(model);
        }
    }
}
