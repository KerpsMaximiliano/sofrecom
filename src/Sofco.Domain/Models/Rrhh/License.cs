using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Rrhh
{
    public class License : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ManagerId { get; set; }
        public User Manager { get; set; }

        public DateTime CreationDate { get; set; }

        public int SectorId { get; set; }
        public Sector Sector { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TypeId { get; set; }
        public LicenseType Type { get; set; }

        public bool WithPayment { get; set; }

        public int DaysQuantity { get; set; }

        public bool HasCertificate { get; set; }

        public bool Parcial { get; set; }

        public bool Final { get; set; }

        public string Comments { get; set; }

        public string ExamDescription { get; set; }

        public LicenseStatus Status { get; set; }

        public ICollection<LicenseFile> LicenseFiles { get; set; }

        public ICollection<LicenseHistory> Histories { get; set; }

        public int DaysQuantityByLaw { get; set; }
    }
}
