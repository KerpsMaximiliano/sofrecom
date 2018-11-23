using System.Collections.Generic;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowService
    {
        Response DoTransition<T>(WorkflowChangeStatusParameters parameters) where T : WorkflowEntity;
        Response<IList<TransitionItemModel>> GetPossibleTransitions<T>(TransitionParameters parameters) where T : WorkflowEntity;
    }
}
