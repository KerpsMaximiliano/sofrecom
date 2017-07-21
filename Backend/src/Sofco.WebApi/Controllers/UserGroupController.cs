using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.WebApi.Models;
using System.Linq;

namespace Sofco.WebApi.Controllers
{
    [Route("api/userGroup")]
    public class UserGroupController : Controller
    {
        private readonly IUserGroupService _userGroupService;
        private readonly IMapper _mapper;

        public UserGroupController(IUserGroupService userGroupsService, IMapper mapper)
        {
            _userGroupService = userGroupsService;
            _mapper = mapper;
        }

        // GET: api/userGroup
        [HttpGet]
        public IActionResult Get()
        {
            var items = _userGroupService.GetAllReadOnlyWithEntitiesRelated();

            var userGroups = items.Select(userGroup => new UserGroupModel(userGroup));

            return Ok(userGroups);
        }

        // GET api/userGroup/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var userGroup = _userGroupService.GetById(id);

            if (userGroup == null) return NotFound();

            var model = new UserGroupModel(userGroup);

            return Ok(model);
        }

        // POST api/userGroup
        [HttpPost]
        public IActionResult Post([FromBody]UserGroupModel model)
        {
            var userGroup = _mapper.Map<UserGroupModel, UserGroup>(model);
            _userGroupService.Insert(userGroup);
            return Ok();
        }

        // PUT api/userGroup
        [HttpPut]
        public IActionResult Put([FromBody]UserGroupModel model)
        {
            var item = _userGroupService.GetById(model.Id);

            if (item == null) return NotFound();

            model.ApplyTo(item);

            _userGroupService.Update(item);

            return Ok();
        }

        // DELETE api/userGroup/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _userGroupService.GetById(id);

            if (item == null) return NotFound();

            _userGroupService.Delete(item);

            return Ok();
        }
    }
}
