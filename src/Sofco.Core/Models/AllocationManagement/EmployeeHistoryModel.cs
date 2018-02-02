using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeHistoryModel
    {
        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public string Seniority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
