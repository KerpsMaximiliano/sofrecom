using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeNewsModel
    {
        public int Id { get; set; }

        public string EmployeeNumber { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public string EmployeeData { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsReincorporation { get; set; }
    }
}
