using System;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundListParameterModel
    {
        public int? UserApplicantId { get; set; }

        public DateTime? DateSince { get; set; }

        public DateTime? DateTo { get; set; }

        public int WorkflowStateId { get; set; }

        public bool InWorkflowProcess { get; set; }
    }
}
