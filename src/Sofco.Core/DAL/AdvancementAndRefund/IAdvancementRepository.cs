using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;

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
        IList<Advancement> GetByApplicant(int id);
        IList<Advancement> GetUnrelated(int currentUserId, int workflowStatusDraftId);
        IList<Advancement> GetAllPaymentPending(int workFlowStatePaymentPending);
    }
}
