using System.Collections.Generic;

namespace Sofco.Core.Models.ManagementReport
{
    public class BillingDetail
    {
        public string ManagerId { get; set; }

        public IList<ProjectOption> Projects { get; set; }

        public IList<MonthBillingHeaderItem> MonthsHeader { get; set; }

        public IList<BillingRowItem> Rows { get; set; }
    }

    public class ProjectOption
    {
        public string Id { get; set; }

        public string OpportunityId { get; set; }

        public string Text { get; set; }
    }

    public class MonthBillingHeaderItem
    {
        public int Month { get; set; }

        public int Year { get; set; }
        public string Display { get; set; }
    }

    public class BillingRowItem
    {
        public string Description { get; set; }

        public IList<MonthBiilingRowItem> MonthValues { get; set; }
    }

    public class MonthBiilingRowItem
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Value { get; set; }
    }
}
