using Sofco.Domain.Models.AllocationManagement;
using System.Collections;
using System.Collections.Generic;

namespace Sofco.Domain.Models.Billing
{
    public class Opportunity
    {
        public int Id { get; set; }

        public string CrmId { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public decimal? ActualValue { get; set; }

        public string ContactId { get; set; }

        public string ParentContactName { get; set; }

        public string ProjectManagerId { get; set; }

        public string ProjectManagerName { get; set; }

        public ICollection<AnalyticsRenovation> AnalyticsRenovations { get; set; }
    }
}
