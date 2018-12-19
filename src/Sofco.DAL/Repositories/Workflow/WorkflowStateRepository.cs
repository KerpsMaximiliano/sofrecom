using System.Collections.Generic;
using Sofco.Core.DAL.Workflow;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Repositories.Workflow
{
    public class WorkflowStateRepository : IWorkflowStateRepository
    {
        protected readonly SofcoContext Context;

        public WorkflowStateRepository(SofcoContext context)
        {
            Context = context;
        }

        public List<WorkflowState> GetStateByWorkflowTypeCode(string workflowTypeCode)
        {
            // TODO: Remove Placeholder

            return new List<WorkflowState>()
            {
                new WorkflowState { Name = "State1" },
                new WorkflowState { Name = "State2" },
            };
        }
    }
}
