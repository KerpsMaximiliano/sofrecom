using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IRefundService
    {
        Response<string> Add(RefundModel model);

        Response<List<WorkflowStateOptionModel>> GetStates();
    }
}
