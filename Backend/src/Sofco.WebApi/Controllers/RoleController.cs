using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.WebApi.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers
{
    [Route("api/role")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUserGroupService _userGroupService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IUserGroupService userGroupsService, IMapper mapper)
        {
            _roleService = roleService;
            _userGroupService = userGroupsService;
            _mapper = mapper;
        }

        // GET: api/role
        [HttpGet]
        public IActionResult Get()
        {
            var items = _roleService.GetAllReadOnly();

            var roles = _mapper.Map<IList<Role>, IList<RoleModel>>(items);

            return Ok(items);
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
            var role = _mapper.Map<RoleModel, Role>(model);

            var response = _roleService.Insert(role);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // PUT api/role
        [HttpPut]
        public IActionResult Put([FromBody]RoleModel model)
        {
            var role = _mapper.Map<RoleModel, Role>(model);

            var response = _roleService.Update(role);

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
