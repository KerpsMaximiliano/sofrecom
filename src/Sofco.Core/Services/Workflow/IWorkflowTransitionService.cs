using Sofco.Core.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Workflow
{
    public interface IWorkflowTransitionService
    {
        Response Post(WorkflowTransitionAddModel model);
        Response<WorkflowTransitionAddModel> Get(int id);
        Response Delete(int id);
        Response Put(WorkflowTransitionAddModel model);
    }
}
