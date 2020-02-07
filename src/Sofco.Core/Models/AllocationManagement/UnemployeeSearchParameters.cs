using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class UnemployeeSearchParameters
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Name { get; set; }

        public string Seniority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public string EmployeeNumber { get; set; }

        public int? AnalyticId { get; set; }

        public int? SuperiorId { get; set; }

        public int? ManagerId { get; set; }
    }

    public class ReportUpdownParameters
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
