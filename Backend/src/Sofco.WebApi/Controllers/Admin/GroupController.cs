using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models.Admin;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Model.Models.Admin;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/group")]
    [Authorize]
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
            var groups = _groupService.GetAllReadOnly(false);
            var model = new List<GroupModel>();

            foreach (var group in groups)
                model.Add(new GroupModel(group));

            return Ok(model);
        }

        // GET: api/group/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var groups = _groupService.GetAllReadOnly(true);
            var model = new List<SelectListItem>();

            foreach (var group in groups)
                model.Add(new SelectListItem { Value = group.Id.ToString(), Text = group.Description });

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
                groupModel.Role = new RoleModel(response.Data.Role);

            foreach (var userGroup in response.Data.UserGroups)
                groupModel.Users.Add(new UserModel(userGroup.User));

            return Ok(groupModel);
        }

        // POST api/group
        [HttpPost]
        public IActionResult Post([FromBody]GroupModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var group = new Group();
            group.Role = new Role();

            model.ApplyTo(group);
            group.Role.Id = model.RoleId;

            var response = _groupService.Insert(group);

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
            var roleId = model.Role != null ? model.Role.Id : 0;

            var response = _groupService.Update(groupReponse.Data, roleId);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = _groupService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
