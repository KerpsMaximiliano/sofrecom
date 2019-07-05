using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Workflow;

namespace Sofco.DAL.Repositories.Workflow
{
    public class WorkflowRepository : IWorkflowRepository
    {
        protected readonly SofcoContext context;

        public WorkflowRepository(SofcoContext context)
        {
            this.context = context;
        }

        public T GetEntity<T>(int id) where T : WorkflowEntity
        {
            return context.Set<T>().Include(x => x.UserApplicant).SingleOrDefault(x => x.Id == id);
        }

        public void UpdateStatus(WorkflowEntity entity)
        {
            context.Entry(entity).Property("StatusId").IsModified = true;
        }

        public void UpdateInWorkflowProcess(WorkflowEntity entity)
        {
            context.Entry(entity).Property("InWorkflowProcess").IsModified = true;
        }

        public void UpdateActive(Domain.Models.Workflow.Workflow entity)
        {
            context.Entry(entity).Property("Active").IsModified = true;
        }

        public bool TransitionExist(int transitionId, int actualStateId, int nextStateId, int workflowId)
        {
            return context.WorkflowStateTransitions.Any(x => x.ActualWorkflowStateId == actualStateId && x.NextWorkflowStateId == nextStateId && x.WorkflowId == workflowId && x.Id != transitionId);
        }

        public void UpdateUsersAlreadyApproved(WorkflowEntity entity)
        {
            context.Entry(entity).Property("UsersAlreadyApproved").IsModified = true;
        }

        public IList<WorkflowStateTransition> GetTransitionByWorkflowAndState(int workflowId, int workflowStatusId)
        {
            return context.WorkflowStateTransitions
                .Include(x => x.WorkflowStateAccesses)
                .ThenInclude(x => x.UserSource)
                .Where(x => x.WorkflowId == workflowId && x.ActualWorkflowStateId == workflowStatusId)
                .ToList();
        }

        public Domain.Models.Workflow.Workflow GetByTypeActive(int workflowTypeId)
        {
            return context.Workflows.SingleOrDefault(x => x.WorkflowTypeId == workflowTypeId && x.Active);
        }

        public WorkflowStateTransition GetTransition(int actualStateId, int nextStateId, int workflowId)
        {
            return context.WorkflowStateTransitions
                .Include(x => x.Workflow)
                .Include(x => x.ActualWorkflowState)
                .Include(x => x.NextWorkflowState)
                .Include(x => x.WorkflowStateAccesses).ThenInclude(x => x.UserSource)
                .Include(x => x.WorkflowStateNotifiers).ThenInclude(x => x.UserSource)
                .SingleOrDefault(x =>
                                 x.WorkflowId == workflowId && 
                                 x.ActualWorkflowStateId == actualStateId &&
                                 x.NextWorkflowStateId == nextStateId);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public Domain.Models.Workflow.Workflow GetById(int workflowId)
        {
            return context.Workflows
                .Include(x => x.Transitions)
                    .ThenInclude(x => x.ActualWorkflowState)
                .Include(x => x.Transitions)
                    .ThenInclude(x => x.NextWorkflowState)
                .SingleOrDefault(x => x.Id == workflowId);
        }

        public bool TypeExist(int type)
        {
            return context.WorkflowTypes.Any(x => x.Id == type);
        }

        public void Add(Domain.Models.Workflow.Workflow domain)
        {
            context.Workflows.Add(domain);
        }

        public IList<WorkflowType> GetTypes()
        {
            return context.WorkflowTypes.ToList().AsReadOnly();
        }

        public bool StateExist(int id)
        {
            return context.WorkflowStates.Any(x => x.Id == id);
        }

        public bool WorkflowExist(int id)
        {
            return context.Workflows.Any(x => x.Id == id);
        }

        public void AddTransition(WorkflowStateTransition domain)
        {
            context.WorkflowStateTransitions.Add(domain);
        }

        public IList<WorkflowState> GetStates()
        {
            return context.WorkflowStates.Where(x => x.Active).ToList().AsReadOnly();
        }

        public WorkflowStateTransition GetTransition(int id)
        {
            return context.WorkflowStateTransitions
                .Include(x => x.WorkflowStateAccesses)
                    .ThenInclude(x => x.UserSource)
                .Include(x => x.WorkflowStateNotifiers)
                    .ThenInclude(x => x.UserSource)
                .SingleOrDefault(x => x.Id == id);
        }

        public WorkflowStateTransition GetTransitionLite(int id)
        {
            return context.WorkflowStateTransitions.Find(id);
        }

        public void DeleteTransition(WorkflowStateTransition transition)
        {
            context.WorkflowStateTransitions.Remove(transition);
        }

        public void UpdateTransition(WorkflowStateTransition transition)
        {
            context.WorkflowStateTransitions.Update(transition);
        }

        public void Update(Domain.Models.Workflow.Workflow domain)
        {
            context.Workflows.Update(domain);
        }

        public Domain.Models.Workflow.Workflow GetLastByType(int typeId)
        {
            var maxVersion = context.Workflows.Where(x => x.WorkflowTypeId == typeId).Max(x => x.Version);

            return context.Workflows.SingleOrDefault(x => x.WorkflowTypeId == typeId && x.Version == maxVersion);
        }

        public int GetVersion(int workflowTypeId)
        {
            var workflows = context.Workflows.Where(x => x.WorkflowTypeId == workflowTypeId).ToList();

            if (!workflows.Any())
            {
                return 1;
            }
            else
            {
                var maxVersion = workflows.Max(x => x.Version);
                maxVersion++;
                return maxVersion;
            }
        }

        public IList<WorkflowStateTransition> GetTransitions(int actualStateId, int workflowId)
        {
            return context.WorkflowStateTransitions
                .Include(x => x.Workflow)
                .Include(x => x.NextWorkflowState)
                .Include(x => x.WorkflowStateAccesses).ThenInclude(x => x.UserSource)
                .Where(x => x.WorkflowId == workflowId && x.ActualWorkflowStateId == actualStateId)
                .ToList();
        }

        public void AddHistory<THistory>(THistory history) where THistory : WorkflowHistory
        {
            context.Set<THistory>().Add(history);
        }

        public IList<Domain.Models.Workflow.Workflow> GetAll()
        {
            return context.Workflows.Include(x => x.ModifiedBy).ToList().AsReadOnly();
        }
    }
}
