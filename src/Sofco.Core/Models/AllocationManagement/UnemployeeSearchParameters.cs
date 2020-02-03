using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class UnemployeeSearchParameters
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Name { get; set; }
    }

    public class ReportUpdownParameters
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
