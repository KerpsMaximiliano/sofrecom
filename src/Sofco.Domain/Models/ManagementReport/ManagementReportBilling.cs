using System;
using System.Collections.Generic;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ManagementReportBilling : BaseEntity
    {
        public int ManagementReportId { get; set; }

        public ManagementReport ManagementReport { get; set; }

        public DateTime MonthYear { get; set; }

        public decimal EvalPropBillingValue { get; set; }
        public decimal EvalPropExpenseValue { get; set; }

        public string Comments { get; set; }

        public int BilledResources { get; set; }

        public decimal EvalPropDifference { get; set; }

        public bool Closed { get; set; }

        public IList<ResourceBilling> ResourceBillings { get; set; }

        public decimal BilledResourceTotal { get; set; }
    }
}
