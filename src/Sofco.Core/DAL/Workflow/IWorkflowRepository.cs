using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.DAL.Workflow
{
    public interface IWorkflowRepository
    {
        T GetEntity<T>(int id) where T : class;
        void UpdateStatus(object entity);
        WorkflowStateTransition GetTransition(int entityStatusId, int parametersNextStateId, int parametersWorkflowId);
        void Save();
    }
}
