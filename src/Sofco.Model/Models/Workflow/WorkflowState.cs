using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Workflow
{
    public class WorkflowState : BaseEntity, IAudit
    {
        public string Name { get; set; }

        public IList<WorkflowStateTransition> ActualTransitions { get; set; }
        public IList<WorkflowStateTransition> NextTransitions { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public int ModifiedById { get; set; }
        public User ModifiedBy { get; set; }
    }
}
