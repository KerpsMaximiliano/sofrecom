using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IAdvancementService
    {
        Response<string> Add(AdvancementModel model);
        Response Update(AdvancementModel model);
        Response<AdvancementEditModel> Get(int id);
        Response<IList<AdvancementListItem>> GetAllInProcess();
        Response<IList<WorkflowHistoryModel>> GetHistories(int id);
        Response<IList<AdvancementListItem>> GetAllFinalized(AdvancementSearchFinalizedModel model);
        Response<bool> CanLoad();
        Response<IList<AdvancementUnrelatedItem>> GetUnrelated();
        Response Delete(int id);
        Response<IList<AdvancementListItem>> GetAllPaymentPending(AdvancementSearchFinalizedModel model);
    }
}
