using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Workflow
{
    public class Workflow : BaseEntity, IAudit
    {
        public string Description { get; set; }

        public string Version { get; set; }

        public bool Active { get; set; }

        public int WorkflowTypeId { get; set; }
        public WorkflowType WorkflowType { get; set; }

        public IList<WorkflowStateTransition> Transitions { get; set; }

        public IList<WorkflowReadAccess> WorkflowReadAccesses { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public int ModifiedById { get; set; }

        public User ModifiedBy { get; set; }
    }
}
