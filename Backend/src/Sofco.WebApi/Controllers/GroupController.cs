using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers
{
    [Route("api/group")]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupsService)
        {
            _groupService = groupsService;
        }

        // GET: api/group
        [HttpGet]
        public IActionResult Get()
        {
            var groups = _groupService.GetAllReadOnly();
            var model = new List<GroupModel>();

            foreach (var group in groups)
            {
                var groupModel = new GroupModel(group);

                if(group.Role != null)
                {
                    groupModel.Role = new RoleModel(group.Role);
                }

                foreach (var userGroup in group.UserGroups)
                {
                    groupModel.Users.Add(new UserModel(userGroup.User));
                }

                model.Add(groupModel);
            }

            return Ok(model);
        }

        // GET api/group/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _groupService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var groupModel = new GroupModel(response.Data);

            if (response.Data.Role != null)
            {
                groupModel.Role = new RoleModel(response.Data.Role);
            }

            foreach (var userGroup in response.Data.UserGroups)
            {
                groupModel.Users.Add(new UserModel(userGroup.User));
            }

            return Ok(groupModel);
        }

        // POST api/group
        [HttpPost]
        public IActionResult Post([FromBody]GroupModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var userGroup = new Group();

            model.ApplyTo(userGroup);

            var response = _groupService.Insert(userGroup);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // PUT api/group
        [HttpPut]
        public IActionResult Put([FromBody]GroupModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var groupReponse = _groupService.GetById(model.Id);

            if (groupReponse.HasErrors()) return BadRequest(groupReponse);

            model.ApplyTo(groupReponse.Data);

            var response = _groupService.Update(groupReponse.Data);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        // DELETE api/group/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = _groupService.DeleteById(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
