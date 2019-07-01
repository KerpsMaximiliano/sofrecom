using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.ManagementReport
{
    public class ManagementReportUpdateDates
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class ManagementReportSendModel
    {
        public int Id { get; set; }

        public ManagementReportStatus? Status { get; set; }
    }

    public class ManagementReportCloseModel
    {
        public int BillingId { get; set; }

        public int DetailCostId { get; set; }

        public DateTime? Date { get; set; }
    }
}
