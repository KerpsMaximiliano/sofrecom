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
        void Update(Domain.Models.Workflow.Workflow domain);
        Domain.Models.Workflow.Workflow GetLastByType(int typeId);
        int GetVersion(int workflowTypeId);
        void UpdateInWorkflowProcess(WorkflowEntity entity);
        Domain.Models.Workflow.Workflow GetByTypeActive(int workflowTypeId);
        void UpdateActive(Domain.Models.Workflow.Workflow entity);
        bool TransitionExist(int transitionId, int actualStateId, int nextStateId, int workflowId);
        void UpdateUsersAlreadyApproved(WorkflowEntity entity);
    }
}
