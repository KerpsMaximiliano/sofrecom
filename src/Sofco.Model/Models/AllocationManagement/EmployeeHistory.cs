using System;
using Sofco.Common.Domains;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class EmployeeHistory : BaseEntity, IEntityDate
    {
        public string EmployeeNumber { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string EmployeeData { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
