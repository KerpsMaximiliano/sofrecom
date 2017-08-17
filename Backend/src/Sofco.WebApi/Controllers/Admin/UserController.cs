using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Core.Services;
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

        public UserController(IUserService userService, IRoleService roleService, IFunctionalityService functionalityService)
        {
            _userService = userService;
            _roleService = roleService;
            _functionalityService = functionalityService;
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
            {
                model.Add(new Option<int>(user.Id, user.Name));
            }

            return Ok(model);
        }

        // GET: api/user
        [HttpGet]
        [Route("full")]
        public IActionResult GetFull()
        {
            var users = _userService.GetAllFullReadOnly();
            var model = new List<UserModel>();

            foreach (var user in users)
            {
                var userModel = new UserModel(user);

                foreach (var userGroup in user.UserGroups)
                {
                    userModel.Groups.Add(new GroupModel(userGroup.Group));
                }

                model.Add(userModel);
            }

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
                    model.Groups.Add(new Option<int>(userGroup.Group.Id, userGroup.Group.Description));
                }

                var roles = _roleService.GetRolesByGroup(response.Data.UserGroups.Select(x => x.GroupId));

                foreach (var rol in roles)
                {
                    if(rol != null)
                    {
                        var roleModel = new Option<int>(rol.Id, rol.Description);
                        if (!model.Roles.Any(x => x.Value == roleModel.Value))
                        {
                            model.Roles.Add(roleModel);
                        }
                    }
                }

                var roleModuleFunctionality = _functionalityService.GetFunctionalitiesByRole(roles.Select(x => x.Id));

                foreach (var roleModuleFunct in roleModuleFunctionality)
                {
                    if(roleModuleFunct.Module != null)
                    {
                        if (!model.Modules.Any(x => x.Code == roleModuleFunct.Module.Code) && roleModuleFunct.Module.Active)
                        {
                            var module = new ModuleModelDetail(roleModuleFunct.Module);

                            model.Modules.Add(module);
                        }
                    }
                }

                for (int i = 0; i < model.Modules.Count(); i++)
                {
                    var roleModuleFunctionalities = roleModuleFunctionality.Where(x => x.Module.Code == model.Modules[i].Code).ToList();

                    model.Modules[i].Functionalities = roleModuleFunctionalities.Where(x => x.Functionality.Active).Select(x => new Option<string>(x.Functionality.Code, x.Functionality.Description)).ToList();
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
