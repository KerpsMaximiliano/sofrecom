namespace Sofco.Core.Models.AdvancementAndRefund.Advancement
{
    public class AdvancementUnrelatedItem
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyText { get; set; }

        public decimal Ammount { get; set; }

        public bool HasLastRefundMarked { get; set; }
    }
}
