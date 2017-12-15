using System;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class EmployeeLicense : BaseEntity, IEntityDate
    {
        public string EmployeeNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int LicenseTypeNumber { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
