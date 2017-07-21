using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;

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
            var items = _userGroupService.GetAllReadOnly();

            return Ok(items);
        }

        // GET api/userGroup/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = _userGroupService.GetById(id);

            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST api/userGroup
        [HttpPost]
        public IActionResult Post([FromBody]UserGroup model)
        {
            return Ok();
        }

        // PUT api/userGroup
        [HttpPut]
        public IActionResult Put([FromBody]UserGroup model)
        {
            return Ok();
        }

        // DELETE api/userGroup/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
