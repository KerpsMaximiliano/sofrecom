using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class BuyOrderProductService : BaseEntity
    {
        public int RequestNoteProductServiceId { get; set; }
        public RequestNoteProductService RequestNoteProductService { get; set; }
        public int BuyOrderId { get; set; }
        public BuyOrder BuyOrder { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int? DeliveredQuantity { get; set; }
    }
}
