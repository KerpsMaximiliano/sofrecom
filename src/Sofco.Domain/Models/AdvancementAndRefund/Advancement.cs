using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class Advancement : WorkflowEntity
    {

        public AdvancementPaymentForm PaymentForm { get; set; }

        public AdvancementType Type { get; set; }

        public int? MonthsReturnId { get; set; }
        public MonthsReturn MonthsReturn { get; set; }

        public DateTime? StartDateReturn { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public DateTime CreationDate { get; set; }

        public string AdvancementReturnForm { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public IList<AdvancementHistory> Histories { get; set; }

        public int? RefundId { get; set; }
        public Refund Refund { get; set; }

        public int WorkflowId { get; set; }
        public Workflow.Workflow Workflow { get; set; }
    }
}
