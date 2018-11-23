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
            context.Entry(entity).Property("InWorkflowProcess").IsModified = true;
        }

        public WorkflowStateTransition GetTransition(int actualStateId, int nextStateId, int workflowId)
        {
            return context.WorkflowStateTransitions
                .Include(x => x.Workflow)
                .Include(x => x.WorkflowStateAccesses).ThenInclude(x => x.UserSource)
                .Include(x => x.WorkflowStateNotifiers).ThenInclude(x => x.UserSource)
                .SingleOrDefault(x => x.Workflow.Active &&
                                 x.WorkflowId == workflowId && 
                                 x.ActualWorkflowStateId == actualStateId &&
                                 x.NextWorkflowStateId == nextStateId);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public IList<WorkflowStateTransition> GetTransitions(int actualStateId, int workflowId)
        {
            return context.WorkflowStateTransitions
                .Include(x => x.Workflow)
                .Include(x => x.NextWorkflowState)
                .Include(x => x.WorkflowStateAccesses).ThenInclude(x => x.UserSource)
                .Where(x => x.Workflow.Active && x.WorkflowId == workflowId && x.ActualWorkflowStateId == actualStateId)
                .ToList();
        }

        public bool IsEndTransition(int actualStateId, int workflowId)
        {
            return !context.WorkflowStateTransitions
                .Include(x => x.Workflow)
                .Any(x => x.Workflow.Active &&
                          x.WorkflowId == workflowId &&
                          x.ActualWorkflowStateId == actualStateId);
        }
    }
}
