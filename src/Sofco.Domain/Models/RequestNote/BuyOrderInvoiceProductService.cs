using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class BuyOrderInvoiceProductService : BaseEntity
    {
        public int BuyOrderProductServiceId { get; set; }
        public BuyOrderProductService BuyOrderProductService { get; set; }
        public int BuyOrderInvoiceId { get; set; }
        public BuyOrderInvoice BuyOrderInvoice { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
