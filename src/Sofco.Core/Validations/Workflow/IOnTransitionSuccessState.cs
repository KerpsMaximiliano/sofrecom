using Sofco.Domain.Interfaces;

namespace Sofco.Core.Validations.Workflow
{
    public interface IOnTransitionSuccessState
    {
        void Process(WorkflowEntity entity);
    }
}
