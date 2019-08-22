using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/profile")]
    public class ProfileController : OptionController<Profile>
    {
        public ProfileController(IOptionService<Profile> service)
            : base(service)
        {
        }
    }
}
