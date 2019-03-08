using Sofco.Core.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowTransitionValidation
    {
        void ValidateAdd(WorkflowTransitionAddModel model, Response response, int transitionId = 0);
    }
}
