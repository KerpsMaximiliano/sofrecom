using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Common;
using Sofco.Core.Services.Common;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers
{
    [Authorize]
    public abstract class OptionController<TEntity> : Controller
        where TEntity : Option
    {
        private readonly IOptionService<TEntity> service;

        protected OptionController(IOptionService<TEntity> service)
        {
            this.service = service;
        } 

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = service.Get();

            return this.CreateResponse(response);
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var response = service.GetActives();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] GenericOptionModel option)
        {
            var response = service.Add(option.Text, option.Parameters);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] GenericOptionModel option)
        {
            var response = service.Update(option.Id, option.Text, option.Parameters);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/active/{active}")]
        public IActionResult Delete(int id, bool active)
        {
            var response = service.Active(id, active);

            return this.CreateResponse(response);
        }
    }
}
