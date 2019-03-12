using System.Collections.Generic;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Utils
{
    public class Currency : Option
    {
        public IList<Solfac> Solfacs { get; set; }

        public string CrmId { get; set; }

        public ICollection<PurchaseOrderAmmountDetail> AmmountDetails { get; set; }

        public ICollection<Advancement> Advancements { get; set; }

        public ICollection<Refund> Refunds { get; set; }
    }
}
