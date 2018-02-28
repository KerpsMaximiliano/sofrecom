using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Admin;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly IFunctionalityService functionalityService;
        private readonly ILoginService loginService;

        public UserController(IUserService userService, IRoleService roleService, IFunctionalityService functionalityService, ILoginService loginServ)
        {
            this.userService = userService;
            this.roleService = roleService;
            this.functionalityService = functionalityService;
            loginService = loginServ;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = userService.GetAllReadOnly(false);

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
            var users = userService.GetAllReadOnly(true);
            var model = new List<UserSelectListItem>();

            foreach (var user in users)
                model.Add(new UserSelectListItem { Value = user.Id.ToString(), Text = user.Name, UserName = user.UserName, Email = user.Email, ExternalId = user.ExternalManagerId });

            return Ok(model);
        }

        // GET api/role/5
        [HttpGet("{id}/detail")]
        public IActionResult Detail(int id)
        {
            var response = userService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            var model = new UserDetailModel(response.Data);

            if (response.Data.UserGroups.Any())
            {
                foreach (var userGroup in response.Data.UserGroups)
                {
                    if (userGroup.Group.Active)
                    {
                        model.Groups.Add(new SelectListItem { Value = userGroup.Group.Id.ToString(), Text = userGroup.Group.Description });
                    }
                }

                var roles = roleService.GetRolesByGroup(model.Groups.Select(x => Convert.ToInt32(x.Value)));

                foreach (var rol in roles)
                {
                    if (rol != null)
                    {
                        var roleModel = new SelectListItem { Value = rol.Id.ToString(), Text = rol.Description };

                        if (!model.Roles.Any(x => x.Value.Equals(roleModel.Value)) && rol.Active)
                        {
                            model.Roles.Add(roleModel);
                        }
                    }
                }

                var roleFunctionalities = functionalityService.GetFunctionalitiesByRole(roles.Select(x => x.Id));
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
            var response = userService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

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
            var response = userService.GetByMail();

            if (response.HasErrors())
                return BadRequest(response);

            var model = new UserModel(response.Data);

            return Ok(model);
        }

        [HttpGet("ad/email/{mail}")]
        public IActionResult AdGetByEmail(string mail)
        {
            var response = userService.CheckIfExist(mail);

            if (response.HasErrors())
                return BadRequest(response);

            var azureResponse = loginService.GetUserFromAzureADByEmail(mail);

            if (azureResponse.HasErrors())
                return BadRequest(azureResponse);

            return Ok(azureResponse);
        }

        [HttpGet("ad/surname/{surname}")]
        public IActionResult AdGetBySurname(string surname)
        {
            var azureResponse = loginService.GetUsersFromAzureADBySurname(surname);

            if (azureResponse.HasErrors())
                return BadRequest(azureResponse);

            return Ok(azureResponse.Data);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var azureResponse = loginService.GetUserFromAzureADByEmail(model.Email);

            if (azureResponse.HasErrors())
                return BadRequest(azureResponse);

            var userAlreadyExistResponse = userService.CheckIfExist(model.Email);

            if (userAlreadyExistResponse.HasErrors())
                return BadRequest(userAlreadyExistResponse);

            var domain = model.CreateDomain();

            var response = userService.Add(domain);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = userService.Active(id, active);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{userId}/group/{groupId}")]
        public IActionResult AddUserGroup(int userId, int groupId)
        {
            var response = userService.AddUserGroup(userId, groupId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{userId}/group/{groupId}")]
        public IActionResult RemoveUserGroup(int userId, int groupId)
        {
            var response = userService.RemoveUserGroup(userId, groupId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [Route("{userId}/groups")]
        public IActionResult ChangeUserGroups(int userId, [FromBody]UserGroupModel model)
        {
            var response = userService.ChangeUserGroups(userId, model.GroupsToAdd, model.GroupsToRemove);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }
    }
}
