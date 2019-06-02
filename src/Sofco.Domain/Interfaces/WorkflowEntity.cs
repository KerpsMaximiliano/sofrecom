using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Domain.Interfaces
{
    public class WorkflowEntity : BaseEntity
    {
        public int UserApplicantId { get; set; }
        public User UserApplicant { get; set; }

        public int StatusId { get; set; }

        public WorkflowState Status { get; set; }

        public bool InWorkflowProcess { get; set; }

        public string UsersAlreadyApproved { get; set; }
    }
}
