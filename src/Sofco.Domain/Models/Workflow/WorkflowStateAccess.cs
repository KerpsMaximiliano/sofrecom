using System;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Workflow
{
    public class WorkflowStateAccess : BaseEntity, IAudit
    {
        public int WorkflowStateTransitionId { get; set; }
        public WorkflowStateTransition WorkflowStateTransition { get; set; }

        public int UserSourceId { get; set; }
        public UserSource UserSource { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public int ModifiedById { get; set; }
        public User ModifiedBy { get; set; }

        public bool AccessDenied { get; set; }

    }
}
