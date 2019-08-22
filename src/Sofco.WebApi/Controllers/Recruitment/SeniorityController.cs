using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/seniority")]
    public class SeniorityController : OptionController<Seniority>
    {
        public SeniorityController(IOptionService<Seniority> service) : base(service)
        {
        }
    }
}
