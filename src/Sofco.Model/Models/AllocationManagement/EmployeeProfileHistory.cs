using System;
using Sofco.Common.Domains;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class EmployeeProfileHistory : BaseEntity, IEntityDate
    {
        public string EmployeeNumber { get; set; }

        public string EmployeeData { get; set; }

        public string EmployeePreviousData { get; set; }

        public string ModifiedFields { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
