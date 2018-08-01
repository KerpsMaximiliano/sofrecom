using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/groups")]
    [Authorize]
    public class GroupController : Controller
    {
        private readonly IGroupService groupService;

        private readonly IMapper mapper;

        public GroupController(IGroupService groupsService, IMapper mapper)
        {
            groupService = groupsService;
            this.mapper = mapper;
        }

        // GET: api/group
        [HttpGet]
        public IActionResult Get()
        {
            var groups = groupService.GetAllReadOnly(false);
            var model = new List<GroupModel>();

            foreach (var group in groups)
                model.Add(new GroupModel(group));

            return Ok(model.OrderBy(x => x.Description));
        }

        // GET: api/group/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var groups = groupService.GetAllReadOnly(true);
            var model = new List<Option>();

            foreach (var group in groups)
                model.Add(new Option { Id = group.Id, Text = group.Description });

            return Ok(model);
        }

        // GET api/group/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = groupService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            var groupModel = new GroupModel(response.Data);

            if (response.Data.Role != null)
                groupModel.Role = new RoleModel(response.Data.Role);

            foreach (var userGroup in response.Data.UserGroups)
                groupModel.Users.Add(Translate(userGroup.User));

            return Ok(groupModel);
        }

        // POST api/group
        [HttpPost]
        public IActionResult Post([FromBody]GroupModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var group = new Group();
            group.Role = new Role();

            model.ApplyTo(group);
            group.Role.Id = model.RoleId;

            var response = groupService.Insert(group);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        // PUT api/group
        [HttpPut]
        public IActionResult Put([FromBody]GroupModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var groupReponse = groupService.GetById(model.Id);

            if (groupReponse.HasErrors())
                return BadRequest(groupReponse);

            model.ApplyTo(groupReponse.Data);

            var response = groupService.Update(groupReponse.Data, model.RoleId);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = groupService.Active(id, active);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        private UserModel Translate(User user)
        {
            return mapper.Map<User, UserModel>(user);
        }
    }
}
