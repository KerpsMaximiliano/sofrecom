using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models;
using Sofco.Core.Services.Admin;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/category")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = categoryService.GetAll(false);

            return Ok(list.Select(x => new CategoryListItem(x)));
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var list = categoryService.GetAll(true);

            return Ok(list.Select(x => new Option { Id = x.Id, Text = x.Description }));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = categoryService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] CategoryModel model)
        {
            var response = categoryService.Add(model.Description);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] CategoryModel model)
        {
            var response = categoryService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = categoryService.Active(id, active);

            return this.CreateResponse(response);
        }
    }
}
