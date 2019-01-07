using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class RejectValidationState : IWorkflowValidationState
    {
        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            if (!parameters.Parameters.ContainsKey("comments"))
            {
                response.AddError(Resources.Workflow.Workflow.CommentsRequired);
                return false;
            }

            return true;
        }
    }
}
