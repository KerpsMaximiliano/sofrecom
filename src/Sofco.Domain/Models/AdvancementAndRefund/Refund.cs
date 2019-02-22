using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class Refund : WorkflowEntity
    {
        public int AnalyticId { get; set; }
        public Analytic Analytic { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal TotalAmmount { get; set; }

        public IList<RefundHistory> Histories { get; set; }

        public IList<RefundDetail> Details { get; set; }

        public IList<RefundFile> Attachments { get; set; }

        public IList<AdvancementRefund> AdvancementRefunds { get; set; }

        public int? CreditCardId { get; set; }

        public CreditCard CreditCard { get; set; }

        public int WorkflowId { get; set; }

        public Workflow.Workflow Workflow { get; set; }

        public bool CashReturn { get; set; }
    }
}
