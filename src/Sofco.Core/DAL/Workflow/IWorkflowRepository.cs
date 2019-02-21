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
        IList<Domain.Models.Workflow.Workflow> GetAll();
        Domain.Models.Workflow.Workflow GetById(int workflowId);
        bool TypeExist(int type);
        void Add(Domain.Models.Workflow.Workflow domain);
        IList<WorkflowType> GetTypes();
        bool StateExist(int id);
        bool WorkflowExist(int id);
        void AddTransition(WorkflowStateTransition domain);
        IList<WorkflowState> GetStates();
        WorkflowStateTransition GetTransition(int id);
        WorkflowStateTransition GetTransitionLite(int id);
        void DeleteTransition(WorkflowStateTransition transition);
        void UpdateTransition(WorkflowStateTransition transition);
    }
}
