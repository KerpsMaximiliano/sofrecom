﻿using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class Currency : Option
    {
        public IList<Solfac> Solfacs { get; set; }

        public IList<Models.Billing.PurchaseOrder> PurchaseOrders { get; set; }

        public string CrmProductionId { get; set; }

        public string CrmDevelopmentId { get; set; }

        public ICollection<PurchaseOrderAmmountDetail> AmmountDetails { get; set; }
    }
}
