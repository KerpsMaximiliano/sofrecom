using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services;
using Sofco.WebApi.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public IActionResult Get()
        {
            var users = _userService.GetAllReadOnly();
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
