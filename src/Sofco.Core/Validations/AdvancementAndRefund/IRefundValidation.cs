using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.AdvancementAndRefund
{
    public interface IRefundValidation
    {
        void ValidateAdd(RefundModel model, Response response);

        Response ValidateUpdate(RefundModel model);
    }
}
