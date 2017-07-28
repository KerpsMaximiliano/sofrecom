using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers
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

        // GET: api/role
        [HttpGet]
        public IActionResult Get()
        {
            var roles = _roleService.GetAllReadOnly();
            var model = new List<RoleModel>();

            foreach (var rol in roles)
                model.Add(new RoleModel(rol));

            return Ok(model);
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

                foreach (var roleFunctionality in rol.RoleFunctionality)
                {
                    roleModel.Functionalities.Add(new FunctionalityModel(roleFunctionality.Functionality));
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
        public IActionResult ChangeFunctionalities(int roleId, [FromBody]RoleFunctionalityModel model)
        {
            var response = _roleService.ChangeFunctionalities(roleId, model.FunctionlitiesToAdd, model.FunctionlitiesToRemove);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{roleId}/menus")]
        public IActionResult ChangeMenus(int roleId, [FromBody]RoleMenuModel model)
        {
            var response = _roleService.ChangeMenus(roleId, model.MenusToAdd, model.MenusToRemove);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
