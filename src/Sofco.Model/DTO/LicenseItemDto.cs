using System;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Domain.DTO
{
    public class LicenseItemDto
    {
        public LicenseItemDto(License license)
        {
            Id = license.Id;
            StartDate = license.StartDate;
            EndDate = license.EndDate;

            if (license.Employee != null)
            {
                EmployeeId = license.EmployeeId;
                EmployeeName = license.Employee.Name;
            }

            if (license.Manager != null)
            {
                ManagerId = license.ManagerId;
                ManagerName = license.Manager.Name;
            }

            if (license.Type != null)
            {
                LicenseTypeName = license.Type.Description;
            }
        }

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public int ManagerId { get; set; }

        public string ManagerName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string LicenseTypeName { get; set; }
    }
}
