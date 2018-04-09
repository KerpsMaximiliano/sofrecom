using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/roles")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService roleService;
        private readonly IGroupService groupService;

        public RoleController(IRoleService roleService, IGroupService groupService)
        {
            this.roleService = roleService;
            this.groupService = groupService;
        }

        // GET: api/role/options
        [HttpGet]
        [Route("options")]
        public IActionResult Getoptions()
        {
            var roles = roleService.GetAllReadOnly(true);
            var model = new List<Option>();

            foreach (var rol in roles)
                model.Add(new Option { Id = rol.Id, Text = rol.Description });

            return Ok(model);
        }

        // GET: api/role
        [HttpGet]
        public IActionResult Get()
        {
            var roles = roleService.GetAllReadOnly(false);
            var model = new List<RoleModel>();

            foreach (var rol in roles)
                model.Add(new RoleModel(rol));

            return Ok(model.OrderBy(x => x.Description));
        }

        // GET: api/role
        [HttpGet]
        [Route("{id}/detail")]
        public IActionResult Detail(int id)
        {
            var response = roleService.GetDetail(id);

            if (response.HasErrors())
                return BadRequest(response);

            var roleModel = new RoleModel(response.Data);

            foreach (var group in response.Data.Groups)
            {
                if (group.Active)
                {
                    roleModel.Groups.Add(new GroupModel(group));
                }
            }

            var modules = response.Data.RoleFunctionality.Select(x => x.Functionality.Module).Distinct().ToList();

            foreach (var module in modules)
            {
                if (module.Active)
                {
                    var moduleModel = new ModuleModel(module);

                    foreach (var functionality in module.Functionalities)
                    {
                        if (functionality.Active)
                            moduleModel.Functionalities.Add(new FunctionalityModel(functionality));
                    }

                    roleModel.Modules.Add(moduleModel);
                }
            }

            return Ok(roleModel);
        }

        // GET api/role/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = roleService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            var roleModel = new RoleModel(response.Data);

            return Ok(roleModel);
        }

        // POST api/role
        [HttpPost]
        public IActionResult Post([FromBody]RoleModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var role = new Role();

            model.ApplyTo(role);

            var response = roleService.Insert(role);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        // PUT api/role
        [HttpPut]
        public IActionResult Put([FromBody]RoleModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var roleReponse = roleService.GetById(model.Id);

            if (roleReponse.HasErrors())
                return BadRequest(roleReponse);

            model.ApplyTo(roleReponse.Data);

            var response = roleService.Update(roleReponse.Data);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{roleId}/group/{groupId}")]
        public IActionResult AddGroup(int roleId, int groupId)
        {
            var response = groupService.AddRole(roleId, groupId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{roleId}/group/{groupId}")]
        public IActionResult RemoveGroup(int roleId, int groupId)
        {
            var response = groupService.RemoveRole(roleId, groupId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = roleService.Active(id, active);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/functionalities")]
        public IActionResult AddFunctionalities(int roleId, [FromBody]List<int> functionalities)
        {
            var response = roleService.AddFunctionalities(roleId, functionalities);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{roleId}/functionalities")]
        public IActionResult RemoveFunctionalities(int roleId, [FromBody]List<int> functionalities)
        {
            var response = roleService.RemoveFunctionalities(roleId, functionalities);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/functionalities/{functionalityId}")]
        public IActionResult AddFunctionality(int roleId, int functionalityId)
        {
            var response = roleService.AddFunctionality(roleId, functionalityId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{roleId}/functionalities/{functionalityId}")]
        public IActionResult DeleteFunctionality(int roleId, int functionalityId)
        {
            var response = roleService.DeleteFunctionality(roleId, functionalityId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }
    }
}
