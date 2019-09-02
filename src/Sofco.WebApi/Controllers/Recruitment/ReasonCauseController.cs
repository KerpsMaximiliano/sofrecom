using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/reasonCause")]
    public class ReasonCauseController : OptionController<ReasonCause>
    {
        public ReasonCauseController(IOptionService<ReasonCause> service)
            : base(service)
        {
        }
    }
}
