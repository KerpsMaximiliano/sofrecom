using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IAdvancementService
    {
        Response<string> Add(AdvancementModel model);
        Response Update(AdvancementModel model);
        Response<AdvancementEditModel> Get(int id);
        Response<IList<WorkflowHistoryModel>> GetHistories(int id);
        Response<IList<AdvancementListItem>> GetAllFinalized(AdvancementSearchFinalizedModel model);
        Response<bool> CanLoad();
        Response<IList<AdvancementUnrelatedItem>> GetUnrelated(int userId);
        Response Delete(int id);
        Response<AdvancementRefundModel> GetResume(IList<int> id);
        Response<IList<Option>> GetStates();
    }
}
