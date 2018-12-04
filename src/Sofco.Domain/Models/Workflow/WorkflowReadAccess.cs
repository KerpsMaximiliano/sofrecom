using System;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Workflow
{
    public class WorkflowReadAccess : BaseEntity, IAudit
    {
        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        public int UserSourceId { get; set; }
        public UserSource UserSource { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public int ModifiedById { get; set; }
        public User ModifiedBy { get; set; }
    }
}
