using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class FinalizeAdvancementWorkflow : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;

        public FinalizeAdvancementWorkflow(IWorkflowManager workflowManager)
        {
            this.workflowManager = workflowManager;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            this.workflowManager.CloseEntity(entity);
        }
    }
}
