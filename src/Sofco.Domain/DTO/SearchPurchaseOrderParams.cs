using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Domain.DTO
{
    public class SearchPurchaseOrderParams
    {
        public string ClientId { get; set; }

        public string StatusId { get; set; }

        public List<PurchaseOrderStatus> StatusIds { get; set; }

        public int? AnalyticId { get; set; }

        public int? ManagerId { get; set; }

        public int? CommercialManagerId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
