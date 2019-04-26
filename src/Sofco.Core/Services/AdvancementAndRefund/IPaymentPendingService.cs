using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IPaymentPendingService
    {
        Response<IList<PaymentPendingModel>> GetAllPaymentPending();
    }
}
