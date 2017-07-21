using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;

namespace Sofco.WebApi.Controllers
{
    [Route("api/role")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/role
        [HttpGet]
        public IActionResult Get()
        {
            var items = _roleService.GetAllReadOnly();

            return Ok(items);
        }

        // GET api/role/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = _roleService.GetById(id);

            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST api/role
        [HttpPost]
        public IActionResult Post([FromBody]Role model)
        {
            _roleService.Insert(model);
            return Ok();
        }

        // PUT api/role
        [HttpPut]
        public IActionResult Put([FromBody]Role model)
        {
            var item = _roleService.GetById(model.Id);

            if (item == null) return NotFound();

            item.Description = model.Description;
            item.Active = model.Active;
            item.Position = model.Position;

            _roleService.Update(item);

            return Ok();
        }

        // DELETE api/role/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _roleService.GetById(id);

            if (item == null) return NotFound();

            _roleService.Delete(item);

            return Ok();
        }
    }
}
