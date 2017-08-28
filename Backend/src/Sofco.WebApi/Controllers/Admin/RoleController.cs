using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using System.Linq;
using Sofco.Model.Models.Admin;

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
                if(group.Active)
                    roleModel.Groups.Add(new GroupModel(group));
            }

            foreach (var roleModule in response.Data.RoleModule)
            {
                if (!roleModel.Modules.Any(x => x.Id == roleModule.ModuleId) && roleModule.Module.Active)
                {
                    var module = new ModuleModel(roleModule.Module);

                    foreach (var moduleFunctionality in roleModule.Module.ModuleFunctionality)
                    {
                        if(moduleFunctionality.Functionality.Active)
                            module.Functionalities.Add(new FunctionalityModel(moduleFunctionality.Functionality));
                    }

                    roleModel.Modules.Add(module);
                }
            }

            return Ok(roleModel);
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

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = _roleService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/modules")]
        public IActionResult AddModules(int roleId, [FromBody]List<int> modules)
        {
            var response = _roleService.ChangeModules(roleId, modules);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/modules/{moduleId}")]
        public IActionResult AddModule(int roleId, int moduleId)
        {
            var response = _roleService.AddModule(roleId, moduleId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{roleId}/modules/{moduleId}")]
        public IActionResult DeleteModule(int roleId, int moduleId)
        {
            var response = _roleService.DeleteModule(roleId, moduleId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
