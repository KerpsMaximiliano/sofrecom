using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.DAL.Workflow
{
    public interface IWorkflowRepository
    {
        T GetEntity<T>(int id) where T : WorkflowEntity;
        void UpdateStatus(WorkflowEntity entity);
        WorkflowStateTransition GetTransition(int entityStatusId, int parametersNextStateId, int parametersWorkflowId);
        void Save();
        IList<WorkflowStateTransition> GetTransitions(int actualStateId, int workflowId);
        bool IsEndTransition(int actualStateId, int workflowId);
        void AddHistory<THistory>(THistory history) where THistory : WorkflowHistory;
    }
}
