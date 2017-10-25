using System;
using System.Collections.Generic;

namespace Sofco.Model.Models.TimeManagement
{
    public class Employee : BaseEntity
    {
        public string EmployeeNumber { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Profile { get; set; }
        public string Technology { get; set; }
        public string Seniority { get; set; }
        public decimal BillingPercentage { get; set; }

        public ICollection<Allocation> Allocations { get; set; }
    }
}
