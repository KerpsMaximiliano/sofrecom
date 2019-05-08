using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.ManagementReport
{
    public class BillingDetail
    {
        public string ManagerId { get; set; }

        public IList<ProjectOption> Projects { get; set; }

        public IList<MonthBillingHeaderItem> MonthsHeader { get; set; }

        public IList<BillingHitoItem> Rows { get; set; }

        public IList<BillingTotal> Totals { get; set; }
    }

    public class ProjectOption
    {
        public string Id { get; set; }

        public string OpportunityId { get; set; }

        public string OpportunityNumber { get; set; }

        public string Text { get; set; }
    }

    public class MonthBillingHeaderItem
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public string Display { get; set; }

        public int ResourceQuantity { get; set; }

        public decimal ValueEvalProp { get; set; }

        public int BillingMonthId { get; set; }
    }

    public class BillingTotal
    {
        public string CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public IList<MonthBiilingRowItem> MonthValues { get; set; }
    }

    public class BillingHitoItem
    {
        public string Description { get; set; }

        public IList<MonthBiilingRowItem> MonthValues { get; set; }

        public string Id { get; set; }

        public string ProjectId { get; set; }

        public DateTime Date { get; set; }

        public string ProjectName { get; set; }

        public string CurrencyId { get; set; }
    }

    public class MonthBiilingRowItem
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Value { get; set; }

        public int SolfacId { get; set; }

        public string Status { get; set; }

        public decimal ValuePesos { get; set; }
    }
}
