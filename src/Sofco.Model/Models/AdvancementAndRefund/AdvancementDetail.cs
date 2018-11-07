using System;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class AdvancementDetail : BaseEntity
    {
        public int AdvancementId { get; set; }
        public Advancement Advancement { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }
    }
}
