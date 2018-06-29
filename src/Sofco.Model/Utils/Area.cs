using System.Collections.Generic;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class Area : Option
    {
        public int ResponsableUserId { get; set; }

        public User ResponsableUser { get; set; }

        public IList<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
