using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IAdvancementRepository : IBaseRepository<Advancement>
    {
        bool Exist(int id);
        Advancement GetById(int id);
        Advancement GetFullById(int id);
        IList<Advancement> GetAllInProcess();
        IList<WorkflowReadAccess> GetWorkflowReadAccess(int advacementWorkflowId);
        IList<AdvancementHistory> GetHistories(int id);
        IList<Advancement> GetAllFinalized(int statusDraft, AdvancementSearchFinalizedModel model);
    }
}
