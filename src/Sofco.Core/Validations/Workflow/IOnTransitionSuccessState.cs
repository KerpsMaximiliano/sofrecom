using Sofco.Core.Models.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Core.Validations.Workflow
{
    public interface IOnTransitionSuccessState
    {
        void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters);
    }
}
