using System.Collections.Generic;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Utils
{
    public class Currency : Option
    {
        public IList<Solfac> Solfacs { get; set; }

        public string CrmProductionId { get; set; }

        public string CrmDevelopmentId { get; set; }

        public ICollection<PurchaseOrderAmmountDetail> AmmountDetails { get; set; }
    }
}
