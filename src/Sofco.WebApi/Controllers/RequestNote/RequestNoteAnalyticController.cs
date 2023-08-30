using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.RequestNote;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteAnalytic")]
    [ApiController]
    public class RequestNoteAnalyticController : Controller
    {
        private readonly IRequestNoteAnalitycService _requestNoteAnalyticService;

        public RequestNoteAnalyticController(IRequestNoteAnalitycService requestNoteAnalyticService)
        {
            this._requestNoteAnalyticService = requestNoteAnalyticService;
        }

        [HttpGet("GetApprovedManageners")]
        public IActionResult GetApprovedManageners(int requestNoteID)
        {
            return Ok(this._requestNoteAnalyticService.GetApprovedManageners(requestNoteID));
        }

    }
}





