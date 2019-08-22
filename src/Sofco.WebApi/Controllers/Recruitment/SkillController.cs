using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/skill")]
    public class SkillController : OptionController<Skill>
    {
        public SkillController(IOptionService<Skill> service) : base(service)
        {
        }
    }
}
