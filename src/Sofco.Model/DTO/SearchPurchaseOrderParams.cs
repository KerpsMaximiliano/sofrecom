using System;

namespace Sofco.Model.DTO
{
    public class SearchPurchaseOrderParams
    {
        public string ClientId { get; set; }

        public string StatusId { get; set; }

        public int? AnalyticId { get; set; }

        public int? ManagerId { get; set; }

        public int? CommercialManagerId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
