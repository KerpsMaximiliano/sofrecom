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
        private readonly IGroupService _userGroupService;
        private readonly IOptions<ActiveDirectoryConfig> _config;

        public RoleController(IRoleService roleService, IGroupService userGroupsService, IOptions<ActiveDirectoryConfig> config)
        {
            _roleService = roleService;
            _userGroupService = userGroupsService;
            _config = config;
        }

        // GET: api/role
        [HttpGet]
        public IActionResult Get()
        {
            var roles = _roleService.GetAllReadOnly();
            var model = new List<RoleModel>();

            foreach (var rol in roles)
            {
                var roleModel = new RoleModel(rol);

                foreach (var group in rol.Groups)
                {
                    roleModel.Groups.Add(new GroupModel(group));
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

            return Ok(response);
        }

        // POST api/role
        [HttpPost]
        public IActionResult Post([FromBody]RoleModel model)
        {
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

        [HttpPost("{roleId}/UserGroup/{userGroupId}")]
        public IActionResult AddUserGroup(int roleId, int userGroupId)
        {
            var response = _userGroupService.AddRole(roleId, userGroupId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
