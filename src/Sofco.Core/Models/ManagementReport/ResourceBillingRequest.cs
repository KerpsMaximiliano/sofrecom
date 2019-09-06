namespace Sofco.Core.Models.ManagementReport
{
    public class ResourceBillingRequest
    {
        public int Id { get; set; }

        public int? ProfileId { get; set; }

        public int? SeniorityId { get; set; }

        public int? PurchaseOrderId { get; set; }

        public int? MonthHour { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public decimal SubTotal { get; set; }

        public bool Deleted { get; set; }
    }
}
