using System;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ManagementReportBilling : BaseEntity
    {
        public int ManagementReportId { get; set; }

        public ManagementReport ManagementReport { get; set; }

        public DateTime MonthYear { get; set; }

        public decimal ValueEvalProp { get; set; }
    }
}
