using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.PurchaseOrders
{
    public class PurchaseOrder
    {
        public int PurchaseOrderNumber { get; set; }
        public int RequestNoteNumber { get; set; }
        public IList<PurchaseOrderItems> Items { get; set; }
    }
}
