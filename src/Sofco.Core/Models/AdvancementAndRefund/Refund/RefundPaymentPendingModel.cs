using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundPaymentPendingModel
    {
        public decimal AdvancementSum { get; set; }

        public decimal RefundItemTotal { get; set; }

        public decimal UserRefund { get; set; }

        public int Id { get; set; }

        public string CurrencyName { get; set; }

        public int UserApplicantId { get; set; }

        public string UserApplicantDesc { get; set; }

        public string Bank { get; set; }

        public DateTime CreationDate { get; set; }

        public int WorkflowId { get; set; }

        public int NextWorkflowStateId { get; set; }
    }
}
