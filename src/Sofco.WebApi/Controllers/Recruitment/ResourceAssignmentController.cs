using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/resourceAssignment")]
    public class ResourceAssignmentController : OptionController<ResourceAssignment>
    {
        public ResourceAssignmentController(IOptionService<ResourceAssignment> service) : base(service)
        {
        }
    }
}
