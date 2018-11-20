using Sofco.Domain.Models.Workflow;

namespace Sofco.Domain.Interfaces
{
    public interface IWorkflowEntity
    {
        int StatusId { get; set; }

        WorkflowState Status { get; set; }
    }
}
