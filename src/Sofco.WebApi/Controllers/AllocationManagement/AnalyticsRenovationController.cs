using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/renovations")]
    public class AnalyticsRenovationController : Controller
    {
        private readonly IAnalyticsRenovationService _analyticsRenovationService;

        public AnalyticsRenovationController(IAnalyticsRenovationService analyticsRenovationService)
        {
            _analyticsRenovationService = analyticsRenovationService;
        }

        [HttpGet("{analyticId}")]
        public IActionResult GetAllByAnalyticId(int analyticId)
        {
            var response = _analyticsRenovationService.GetAllByAnalyticId(analyticId);
            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AnalyticsRenovationModel model)
        {
            var response = _analyticsRenovationService.Add(model.CreateDomain());
            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] AnalyticsRenovationModel model)
        {
            var response = _analyticsRenovationService.Update(model);

            return this.CreateResponse(response);
        }

    }
}
