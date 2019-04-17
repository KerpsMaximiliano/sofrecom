using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class EmployeeRefundDetail
    {
        public int Id { get; set; }

        public string CurrencyName { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal RefundItemTotal { get; set; }

        public WorkflowStateType? WorkflowStatusType { get; set; }

        public string StatusName { get; set; }

        public decimal AdvancementSum { get; set; }

        public bool IsCreditCard { get; set; }

        public bool IsCashReturn { get; set; }
    }
}
