using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Providers;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services;
using Sofco.Core.Services.Providers;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers.Providers
{
    [Route("api/providers")]
    public class ProvidersController : Controller
    {
        private readonly IprovidersService providers;

        public ProvidersController(IprovidersService providers)
        {
            this.providers = providers;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = providers.GetAll();

            return this.CreateResponse(response);
        }

        [HttpPost("GetByParams")]
        public IActionResult GetByParams([FromBody] ProvidersGetByParamsModel param)
        {
            var response = providers.GetByParams(param);

            return this.CreateResponse(response);
        }

        [HttpGet("{providersId}")]
        public IActionResult Get(int providersid)
        {
            var response = providers.GetById(providersid);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProvidersModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);

            var response = providers.Put(id, model);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProvidersModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors())
                return BadRequest(errors);


            var response = providers.Post(model);

            return this.CreateResponse(response);
        }
    }
}
