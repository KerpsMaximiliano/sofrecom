using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundListResultModel
    {
        public int Id { get; set; }

        public int UserApplicantId { get; set; }

        public string UserApplicantName { get; set; }

        public string ManagerName { get; set; }

        public string CurrencyName { get; set; }

        public DateTime CreationDate { get; set; }

        public WorkflowStateType? WorkflowStatusType { get; set; }

        public string StatusName{ get; set; }

        public decimal RefundItemTotal { get; set; }

        public decimal AdvancementSum { get; set; }

        public string Bank { get; set; }
    }
}
