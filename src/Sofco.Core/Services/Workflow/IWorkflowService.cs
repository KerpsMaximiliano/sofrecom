using Sofco.Core.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowService
    {
        Response DoTransition<T>(WorkflowChangeStatusParameters parameters) where T : class;
    }
}
