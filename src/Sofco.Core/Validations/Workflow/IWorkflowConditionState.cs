using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowConditionState
    {
        bool CanDoTransition(WorkflowEntity entity, Response response);
    }
}
