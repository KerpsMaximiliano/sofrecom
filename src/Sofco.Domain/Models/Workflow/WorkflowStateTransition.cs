using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Workflow
{
    public class WorkflowStateTransition: BaseEntity, IAudit
    {
        public WorkflowState ActualWorkflowState { get; set; }
        public int ActualWorkflowStateId { get; set; }

        public WorkflowState NextWorkflowState { get; set; }
        public int NextWorkflowStateId { get; set; }

        public int WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        public IList<WorkflowStateAccess> WorkflowStateAccesses { get; set; }

        public IList<WorkflowStateNotifier> WorkflowStateNotifiers { get; set; }

        public string NotificationCode { get; set; }

        public string ValidationCode { get; set; }

        public string ConditionCode { get; set; }

        public string ParameterCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public int ModifiedById { get; set; }
        public User ModifiedBy { get; set; }
    }
}
