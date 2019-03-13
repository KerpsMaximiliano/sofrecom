using Sofco.Domain.Enums;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundRelatedModel
    {
        public int Id { get; set; }

        public decimal Total { get; set; }

        public string Analytic { get; set; }


        public string StatusName { get; set; }

        public WorkflowStateType? StatusType { get; set; }

        public bool LastRefund { get; set; }
    }
}
