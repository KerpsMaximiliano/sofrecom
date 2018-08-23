using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.WorkTimeManagement
{
    [Route("api/holidays")]
    [Authorize]
    public class HolidaysController : Controller
    {
        private readonly IHolidayService service;

        public HolidaysController(IHolidayService service)
        {
            this.service = service;
        }

        [HttpGet("{year}")]
        public IActionResult Get(int year)
        {
            var response = service.Get(year);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Holiday model)
        {
            var response = service.Post(model);

            return this.CreateResponse(response);
        }

        [HttpPost("importExternalData/{year}")]
        public IActionResult ImportExternalData(int year)
        {
            var response = service.ImportExternalData(year);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = service.Delete(id);

            return this.CreateResponse(response);
        }
    }
}
