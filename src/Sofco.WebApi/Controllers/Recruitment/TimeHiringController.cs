using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/timeHiring")]
    public class TimeHiringController : OptionController<TimeHiring>
    {
        public TimeHiringController(IOptionService<TimeHiring> service)
            : base(service)
        {
        }
    }
}
