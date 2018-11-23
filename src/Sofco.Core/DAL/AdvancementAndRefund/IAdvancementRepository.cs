using System.Collections.Generic;
using Sofco.Core.DAL.Common;
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
    }
}
