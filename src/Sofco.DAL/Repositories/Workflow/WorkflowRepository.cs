using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Workflow;
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

        public T GetEntity<T>(int id) where T : class
        {
            return context.Set<T>().Find(id);
        }

        public void UpdateStatus(object entity)
        {
            context.Entry(entity).Property("StatusId").IsModified = true;
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
    }
}
