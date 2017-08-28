using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IFunctionalityService _functionalityService;
        private readonly IModuleService _moduleService;

        public UserController(IUserService userService, IRoleService roleService, IFunctionalityService functionalityService, IModuleService moduleService)
        {
            _userService = userService;
            _roleService = roleService;
            _functionalityService = functionalityService;
            _moduleService = moduleService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userService.GetAllReadOnly(false);

            var model = new List<UserModel>();

            foreach (var user in users)
                model.Add(new UserModel(user));

            return Ok(model);
        }

        // GET: api/user/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var users = _userService.GetAllReadOnly(true);
            var model = new List<Option<int>>();

            foreach (var user in users)
                model.Add(new Option<int>(user.Id, user.Name));

            return Ok(model);
        }

        // GET api/role/5
        [HttpGet("{id}/detail")]
        public IActionResult Detail(int id)
        {
            var response = _userService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new UserDetailModel(response.Data);

            if (response.Data.UserGroups.Any())
            {
                foreach (var userGroup in response.Data.UserGroups)
                {
                    if(userGroup.Group.Active)
                        model.Groups.Add(new Option<int>(userGroup.Group.Id, userGroup.Group.Description));
                }

                var roles = _roleService.GetRolesByGroup(model.Groups.Select(x => x.Value));

                foreach (var rol in roles)
                {
                    if(rol != null)
                    {
                        var roleModel = new Option<int>(rol.Id, rol.Description);
                        if (!model.Roles.Any(x => x.Value == roleModel.Value) && rol.Active)
                        {
                            model.Roles.Add(roleModel);
                        }
                    }
                }

                var modules = _moduleService.GetModulesByRole(model.Roles.Select(x => x.Value));

                foreach (var module in modules)
                {
                    var moduleModel = new ModuleModelDetail(module);

                    var functionalities = _functionalityService.GetFunctionalitiesByModule(module.Id);

                    moduleModel.Functionalities = functionalities.Select(x => new Option<string>(x.Code, x.Description)).ToList();

                    model.Modules.Add(moduleModel);
                }
            }

            return Ok(model);
        }

        // GET api/role/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _userService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new UserModel(response.Data);

            foreach (var userGroup in response.Data.UserGroups)
            {
                model.Groups.Add(new GroupModel(userGroup.Group));
            }

            return Ok(model);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = _userService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{userId}/group/{groupId}")]
        public IActionResult AddUserGroup(int userId, int groupId)
        {
            var response = _userService.AddUserGroup(userId, groupId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{userId}/group/{groupId}")]
        public IActionResult RemoveUserGroup(int userId, int groupId)
        {
            var response = _userService.RemoveUserGroup(userId, groupId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{userId}/groups")]
        public IActionResult ChangeUserGroups(int userId, [FromBody]UserGroupModel model)
        {
            var response = _userService.ChangeUserGroups(userId, model.GroupsToAdd, model.GroupsToRemove);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
