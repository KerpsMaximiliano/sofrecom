using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class BuyOrderInvoice : BaseEntity
    {
        
        public int BuyOrderId { get; set; }
        public BuyOrder BuyOrder { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string TaxCode { get; set; }
        public int? FileId { get; set; }
        public File File { get; set; }

        public IList<BuyOrderInvoiceProductService> ProductsServices { get; set; }
    }
}
