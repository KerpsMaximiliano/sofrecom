using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/role")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IGroupService _groupService;

        public RoleController(IRoleService roleService, IGroupService groupService)
        {
            _roleService = roleService;
            _groupService = groupService;
        }

        // GET: api/role/options
        [HttpGet]
        [Route("options")]
        public IActionResult Getoptions()
        {
            var roles = _roleService.GetAllReadOnly(true);
            var model = new List<Option<int>>();

            foreach (var rol in roles)
                model.Add(new Option<int>(rol.Id, rol.Description));

            return Ok(model);
        }

        // GET: api/role
        [HttpGet]
        public IActionResult Get()
        {
            var roles = _roleService.GetAllReadOnly(false);
            var model = new List<RoleModel>();

            foreach (var rol in roles)
                model.Add(new RoleModel(rol));

            return Ok(model);
        }

        // GET: api/role
        [HttpGet]
        [Route("{id}/detail")]
        public IActionResult Detail(int id)
        {
            var response = _roleService.GetDetail(id);

            if (response.HasErrors()) return BadRequest(response);

            var roleModel = new RoleModel(response.Data);

            foreach (var group in response.Data.Groups)
            {
                roleModel.Groups.Add(new GroupModel(group));
            }

            foreach (var roleModuleFunct in response.Data.RoleModuleFunctionality)
            {
                if (!roleModel.Modules.Any(x => x.Id == roleModuleFunct.ModuleId))
                {
                    var module = new ModuleModel(roleModuleFunct.Module);

                    roleModel.Modules.Add(module);
                }
            }

            for (int i = 0; i < roleModel.Modules.Count(); i++)
            {
                var roleModuleFunctionalities = response.Data.RoleModuleFunctionality.Where(x => x.ModuleId == roleModel.Modules[i].Id).ToList();

                roleModel.Modules[i].Functionalities = roleModuleFunctionalities.Select(x => new FunctionalityModel(x.Functionality)).ToList();
            }

            return Ok(roleModel);
        }

        // GET: api/role
        [HttpGet]
        [Route("full")]
        public IActionResult GetFull()
        {
            var roles = _roleService.GetAllFullReadOnly();
            var model = new List<RoleModel>();

            foreach (var rol in roles)
            {
                var roleModel = new RoleModel(rol);

                foreach (var group in rol.Groups)
                {
                    roleModel.Groups.Add(new GroupModel(group));
                }

                foreach (var roleModuleFunct in rol.RoleModuleFunctionality)
                {
                    if(!roleModel.Modules.Any(x => x.Id == roleModuleFunct.ModuleId))
                    {
                        var module = new ModuleModel(roleModuleFunct.Module);

                        roleModel.Modules.Add(module);
                    }
                }

                for (int i = 0; i < roleModel.Modules.Count(); i++)
                {
                    var roleModuleFunctionalities = rol.RoleModuleFunctionality.Where(x => x.ModuleId == roleModel.Modules[i].Id).ToList();

                    roleModel.Modules[i].Functionalities = roleModuleFunctionalities.Select(x => new FunctionalityModel(x.Functionality)).ToList();
                }

                model.Add(roleModel);
            }

            return Ok(model);
        }

        // GET api/role/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _roleService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var roleModel = new RoleModel(response.Data);

            return Ok(roleModel);
        }

        // POST api/role
        [HttpPost]
        public IActionResult Post([FromBody]RoleModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var role = new Role();

            model.ApplyTo(role);

            var response = _roleService.Insert(role);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // PUT api/role
        [HttpPut]
        public IActionResult Put([FromBody]RoleModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var roleReponse = _roleService.GetById(model.Id);

            if (roleReponse.HasErrors()) return BadRequest(roleReponse);

            model.ApplyTo(roleReponse.Data);

            var response = _roleService.Update(roleReponse.Data);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // DELETE api/role/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _roleService.DeleteById(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{roleId}/group/{groupId}")]
        public IActionResult AddGroup(int roleId, int groupId)
        {
            var response = _groupService.AddRole(roleId, groupId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{roleId}/group/{groupId}")]
        public IActionResult RemoveGroup(int roleId, int groupId)
        {
            var response = _groupService.RemoveRole(roleId, groupId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/functionalities")]
        public IActionResult ChangeFunctionalities(int roleId, [FromBody]RoleModuleFunctionalityModel model)
        {
            var response = _roleService.ChangeFunctionalities(roleId, model.ModuleId, model.FunctionalitiesToAdd, model.FunctionalitiesToRemove);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/module/{moduleId}/functionality/{functionalityId}")]
        public IActionResult AddFunctionality(int roleId, int moduleId, int functionalityId)
        {
            var response = _roleService.AddFunctionality(roleId, moduleId, functionalityId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{roleId}/module/{moduleId}/functionality/{functionalityId}")]
        public IActionResult DeleteFunctionality(int roleId, int moduleId, int functionalityId)
        {
            var response = _roleService.DeleteFunctionality(roleId, moduleId, functionalityId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = _roleService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
