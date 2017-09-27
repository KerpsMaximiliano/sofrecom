using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Admin;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/functionality")]
    [Authorize]
    public class FunctionalityController : Controller
    {
        private readonly IFunctionalityService functionalityService;

        public FunctionalityController(IFunctionalityService functionalityService)
        {
            this.functionalityService = functionalityService;
        }

        // GET: api/functionality
        [HttpGet]
        public IActionResult Get()
        {
            var functionalities = functionalityService.GetAllReadOnly(false);
            var model = new List<FunctionalityModel>();

            foreach (var functionality in functionalities)
                model.Add(new FunctionalityModel(functionality));

            return Ok(model.OrderBy(x => x.Description));
        }

        // GET: api/functionality/options
        [HttpGet]
        [Route("options")]
        public IActionResult GetOptions()
        {
            var functionalities = functionalityService.GetAllReadOnly(true);
            var model = new List<SelectListItem>();

            foreach (var functionality in functionalities)
                model.Add(new SelectListItem { Value = functionality.Id.ToString(), Text = functionality.Description });

            return Ok(model);
        }

        // GET api/functionality/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = functionalityService.GetById(id);

            if (response.HasErrors()) return BadRequest(response);

            var model = new FunctionalityModel(response.Data);

             return Ok(model);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = functionalityService.Active(id, active);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
