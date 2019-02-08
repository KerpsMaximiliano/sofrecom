namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class AdvancementRefund
    {
        public int AdvancementId { get; set; }

        public Advancement Advancement { get; set; }

        public int RefundId { get; set; }

        public Refund Refund { get; set; }

        public decimal OriginalAdvancement { get; set; }

        public decimal DiscountedFromAdvancement { get; set; }
    }
}
