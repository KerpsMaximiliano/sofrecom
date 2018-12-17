using System;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class RefundDetail : BaseEntity
    {
        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public int RefundId { get; set; }

        public Refund Refund { get; set; }
    }
}
