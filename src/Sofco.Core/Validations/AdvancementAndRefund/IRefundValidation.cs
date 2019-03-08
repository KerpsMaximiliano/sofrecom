using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.AdvancementAndRefund
{
    public interface IRefundValidation
    {
        void ValidateAdd(RefundModel model, Response response);

        Response ValidateUpdate(RefundModel model);

        bool HasUserRefund(Refund refund);
    }
}
