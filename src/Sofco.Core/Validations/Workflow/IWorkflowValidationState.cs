using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowValidationState
    {
        bool Validate(WorkflowEntity entity, Response response);
    }
}
