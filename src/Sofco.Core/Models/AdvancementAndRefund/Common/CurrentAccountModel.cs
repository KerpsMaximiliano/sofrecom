using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;

namespace Sofco.Core.Models.AdvancementAndRefund.Common
{
    public class CurrentAccountModel
    {
        public string User { get; set; }

        public string Currency { get; set; }

        public decimal AdvancementTotal { get; set; }

        public decimal RefundTotal { get; set; }

        public decimal UserRefund => RefundTotal - AdvancementTotal > 0 ? RefundTotal - AdvancementTotal : 0;

        public decimal CompanyRefund => AdvancementTotal - RefundTotal > 0 ? AdvancementTotal - RefundTotal : 0;

        public int CurrencyId { get; set; }

        public int UserId { get; set; }

        public IList<CurrentAccountRefundModel> Refunds { get; set; }

        public IList<AdvancementUnrelatedItem> Advancements { get; set; }
    }

    public class CurrentAccountRefundModel
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public string Advancements { get; set; }
    }
}
