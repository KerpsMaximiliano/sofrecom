using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Utils
{
    public class Area : Option, ILogicalDelete
    {
        public int ResponsableUserId { get; set; }

        public User ResponsableUser { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
