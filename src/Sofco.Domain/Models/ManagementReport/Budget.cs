using System;

namespace Sofco.Domain.Models.ManagementReport
{
    public class Budget : BaseEntity
    {
        public string Description { get; set; }

        public decimal Value { get; set; }

        public DateTime StartDate { get; set; }

        public int ManagementReportId { get; set; }
        public ManagementReport ManagementReport { get; set; }
    }
}
