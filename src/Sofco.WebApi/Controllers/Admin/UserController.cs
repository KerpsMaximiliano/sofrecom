using System;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/users")]
    //[Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IFunctionalityService _functionalityService;
        private readonly ILoginService loginService;

        public UserController(IUserService userService, IRoleService roleService, IFunctionalityService functionalityService, ILoginService loginServ)
        {
            _userService = userService;
            _roleService = roleService;
            _functionalityService = functionalityService;
            loginService = loginServ;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userService.GetAllReadOnly(false);

            var model = new List<UserModel>();

            foreach (var user in users)
                model.Add(new UserModel(user));

            return Ok(model.OrderBy(x => x.Name));
        }

        // GET: api/user/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var users = _userService.GetAllReadOnly(true);
            var model = new List<UserSelectListItem>();

            foreach (var user in users)
                model.Add(new UserSelectListItem { Value = user.Id.ToString(), Text = user.Name, UserName = user.UserName });

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
                        model.Groups.Add(new SelectListItem { Value = userGroup.Group.Id.ToString(), Text = userGroup.Group.Description });
                }

                var roles = _roleService.GetRolesByGroup(model.Groups.Select(x => Convert.ToInt32(x.Value)));

                foreach (var rol in roles)
                {
                    if(rol != null)
                    {
                        var roleModel = new SelectListItem {Value = rol.Id.ToString(), Text = rol.Description };
                        if (!model.Roles.Any(x => x.Value.Equals(roleModel.Value)) && rol.Active)
                        {
                            model.Roles.Add(roleModel);
                        }
                    }
                }

                var roleFunctionalities = _functionalityService.GetFunctionalitiesByRole(roles.Select(x => x.Id));
                var modules = roleFunctionalities.Select(x => x.Functionality.Module).Distinct();

                foreach (var module in modules)
                {
                    var moduleModel = new ModuleModelDetail(module);

                    moduleModel.Functionalities = module.Functionalities.Select(x => new SelectListItem { Value = x.Code, Text = x.Description }).ToList();

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

        [HttpGet("email/{mail}")]
        public IActionResult Get(string mail)
        {
            var response = _userService.GetByMail(mail);

            if (response.HasErrors()) return BadRequest(response);

            var model = new UserModel(response.Data);

            return Ok(model);
        }

        [HttpGet("ad/{mail}")]
        public IActionResult Exist(string mail)
        {
            var response = _userService.CheckIfExist(mail);

            if (response.HasErrors()) return BadRequest(response);

            var azureResponse = loginService.GetUserFromAzureAD(mail);

            if (azureResponse.HasErrors()) return BadRequest(azureResponse);

            return Ok(azureResponse);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var azureResponse = loginService.GetUserFromAzureAD(model.Email);

            if (azureResponse.HasErrors()) return BadRequest(azureResponse);

            var domain = model.CreateDomain();

            var response = _userService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
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
