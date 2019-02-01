using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IAdvancementRefundSettingService
    {
        Response Save(SettingModel model);

        Response<SettingModel> Get();
    }
}
