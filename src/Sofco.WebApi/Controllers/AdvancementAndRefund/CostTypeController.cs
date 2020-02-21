using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/costType")]
    public class CostTypeController : OptionController<CostType>
    {
        public CostTypeController(IOptionService<CostType> service)
            : base(service)
        {
        }
    }
}
