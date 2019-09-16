using System.Collections.Generic;

namespace Sofco.Core.Models.ManagementReport
{
    public class ResourceBillingRequestItem
    {
        public int Id { get; set; }

        public string Profile { get; set; }

        public int? SeniorityId { get; set; }

        public int? EmployeeId { get; set; }

        public int? MonthHour { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public decimal SubTotal { get; set; }

        public bool Deleted { get; set; }
    }

    public class UpdateResourceBillingRequest
    {
        public string Id { get; set; }

        public string ProjectId { get; set; }

        public decimal? Ammount { get; set; }

        public string Name { get; set; }

        public int Month { get; set; }

        public int BillingMonthId { get; set; }

        public IList<ResourceBillingRequestItem> Resources { get; set; }
    }
}
