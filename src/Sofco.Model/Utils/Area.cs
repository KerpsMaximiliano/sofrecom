using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class Area : Option
    {
        public IList<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
