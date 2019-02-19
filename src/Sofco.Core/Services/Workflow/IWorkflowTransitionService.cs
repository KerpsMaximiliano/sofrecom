using Sofco.Core.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowTransitionService
    {
        Response Post(WorkflowTransitionAddModel model);
    }
}
