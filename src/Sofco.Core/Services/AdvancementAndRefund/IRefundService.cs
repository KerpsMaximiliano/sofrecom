using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IRefundService
    {
        Response<string> Add(RefundModel model);
    }
}
