using System;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class EmployeeSyncAction : BaseEntity, IEntityDate
    {
        public string EmployeeNumber { get; set; }

        public string Status { get; set; }

        public string EmployeeData { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
