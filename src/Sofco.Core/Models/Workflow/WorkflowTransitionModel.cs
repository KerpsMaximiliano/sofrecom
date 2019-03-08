using Sofco.Domain.Models.Workflow;

namespace Sofco.Core.Models.Workflow
{
    public class WorkflowTransitionModel
    {
        public WorkflowTransitionModel(WorkflowStateTransition domain)
        {
            Id = domain.Id;
            ActualWorkflowStateDescription = domain.ActualWorkflowState?.Name;
            NextWorkflowStateDescription = domain.NextWorkflowState?.Name;
            NotificationCode = domain.NotificationCode;
            ValidationCode = domain.ValidationCode;
            ConditionCode = domain.ConditionCode;
            OnSuccessCode = domain.OnSuccessCode;
        }

        public int Id { get; set; }

        public string ActualWorkflowStateDescription { get; set; }

        public string NextWorkflowStateDescription { get; set; }

        public string NotificationCode { get; set; }

        public string ValidationCode { get; set; }

        public string ConditionCode { get; set; }

        public string OnSuccessCode { get; set; }
    }
}
