using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowNotification
    {
        void Send(WorkflowEntity entity, Response response, WorkflowStateTransition transition);
    }
}
