using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class Refund : WorkflowEntity
    {
        public string Contract { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal TotalAmmount { get; set; }

        public IList<RefundHistory> Histories { get; set; }

        public IList<RefundDetail> Details { get; set; }

        public IList<RefundFile> Attachments { get; set; }

        public IList<AdvancementRefund> AdvancementRefunds { get; set; }
    }
}
