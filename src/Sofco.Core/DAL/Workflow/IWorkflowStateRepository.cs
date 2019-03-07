using System.Collections.Generic;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.DAL.Workflow
{
    public interface IWorkflowStateRepository
    {
        List<WorkflowState> GetStateByWorkflowTypeCode(string workflowTypeCode);
        IList<WorkflowState> GetAll();
    }
}