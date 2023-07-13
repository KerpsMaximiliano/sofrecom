using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;
using System.Collections.Generic;

namespace Sofco.WebApi.Controllers.Providers
{
    [Route("api/providersArea")]
    public class ProvidersAreaController : Controller
    {
        private readonly IProvidersAreaService providers;

        public ProvidersAreaController(IProvidersAreaService providersArea)
        {
            this.providers = providersArea;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = providers.GetAll();

            return this.CreateResponse(response);
        }

        [HttpGet("{providersAreaId}")]
        public IActionResult Get(int providersAreaid)
        {
            var response = providers.GetById(providersAreaid);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProvidersAreaModel model)
        {
            var response = providers.Put(id, model);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProvidersAreaModel model)
        {
            var response = providers.Post(model);

            return this.CreateResponse(response);
        }


        [HttpPut("{id}")]
        public IActionResult Enable(int id)
        {
            var response = providers.Enable(id);

            return this.CreateResponse(response);
        }


        [HttpPut("{id}")]
        public IActionResult Disable(int id)
        {
            var response = providers.Disable(id);

            return this.CreateResponse(response);
        }

    }
}
