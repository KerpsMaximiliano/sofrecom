using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.DAL.Workflow
{
    public interface IWorkflowStateRepository : IBaseRepository<WorkflowState>
    {
        List<WorkflowState> GetStateByWorkflowTypeCode(string workflowTypeCode);
        IList<WorkflowState> GetAll();
    }
}