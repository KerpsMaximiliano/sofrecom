using System.Collections.Generic;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowService
    {
        Response DoTransition<TEntity, THistory>(WorkflowChangeStatusParameters parameters) where TEntity : WorkflowEntity where THistory : WorkflowHistory;

        Response<IList<TransitionItemModel>> GetPossibleTransitions<T>(TransitionParameters parameters) where T : WorkflowEntity;

        Response<IList<WorkflowListItemModel>> GetAll();

        Response<WorkflowDetailModel> GetById(int workflowId);
    }
}
