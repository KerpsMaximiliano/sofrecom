using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Workflow;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Repositories.Workflow
{
    public class WorkflowStateRepository : BaseRepository<WorkflowState>, IWorkflowStateRepository
    {
        protected readonly SofcoContext Context;

        public WorkflowStateRepository(SofcoContext context) : base(context)
        {
            Context = context;
        }

        public List<WorkflowState> GetStateByWorkflowTypeCode(string workflowTypeCode)
        {
            var workflowTypeId = Context
                .Set<WorkflowType>()
                .First(s => s.Code == workflowTypeCode)
                .Id;

            var workflowId = Context.Set<Domain.Models.Workflow.Workflow>()
                .First(x => x.WorkflowTypeId == workflowTypeId && x.Active)
                .Id;

            var workflowStateTransitions = Context.Set<WorkflowStateTransition>()
                .Where(s => s.WorkflowId == workflowId);

            var stateIds = workflowStateTransitions.Select(s => s.ActualWorkflowStateId)
                .Distinct()
                .ToList();

            return Context.Set<WorkflowState>()
                .Where(s => stateIds.Contains(s.Id))
                .ToList();
        }

        public new IList<WorkflowState> GetAll()
        {
            return Context.WorkflowStates.ToList().AsReadOnly();
        }
    }
}
