using Sofco.Domain.Interfaces;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowManager
    {
        void CloseAdvancementsAndRefunds(int entityId);
        void CloseEntity(WorkflowEntity entity);
    }
}
