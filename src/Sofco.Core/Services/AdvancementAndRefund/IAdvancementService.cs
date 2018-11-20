using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IAdvancementService
    {
        Response Add(AdvancementModel model);
        Response Update(AdvancementModel model);
        Response<AdvancementEditModel> Get(int id);
    }
}
