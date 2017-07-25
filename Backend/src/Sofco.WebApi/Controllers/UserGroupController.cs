using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.WebApi.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers
{
    [Route("api/userGroup")]
    public class UserGroupController : Controller
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(IUserGroupService userGroupsService)
        {
            _userGroupService = userGroupsService;
        }

        // GET: api/userGroup
        [HttpGet]
        public IActionResult Get()
        {
            var userGroups = _userGroupService.GetAllReadOnly();
            var model = new List<UserGroupModel>();

            foreach (var userGroup in userGroups)
            {
                var userGroupModel = new UserGroupModel(userGroup);
                userGroupModel.Role = new RoleModel(userGroup.Role);
                model.Add(userGroupModel);
            }

            return Ok(model);
        }

        // GET api/userGroup/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _userGroupService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // POST api/userGroup
        [HttpPost]
        public IActionResult Post([FromBody]UserGroupModel model)
        {
            var userGroup = new UserGroup();

            model.ApplyTo(userGroup);

            var response = _userGroupService.Insert(userGroup);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // PUT api/userGroup
        [HttpPut]
        public IActionResult Put([FromBody]UserGroupModel model)
        {
            var userGroupReponse = _userGroupService.GetById(model.Id);

            if (userGroupReponse.HasErrors()) return BadRequest(userGroupReponse);

            model.ApplyTo(userGroupReponse.Data);

            var response = _userGroupService.Update(userGroupReponse.Data);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // DELETE api/userGroup/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _userGroupService.DeleteById(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
