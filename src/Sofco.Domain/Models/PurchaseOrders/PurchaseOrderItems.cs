using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.PurchaseOrders
{
    public class PurchaseOrderItems
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
