using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.Validations
{
    public class WorkflowValidationStateFactory : IWorkflowValidationStateFactory
    {
        public IWorkflowValidationState GetInstance(string code)
        {
            switch (code)
            {
                case "REJECT": return new RejectValidationState();
                default: return null;
            }
        }
    }
}
