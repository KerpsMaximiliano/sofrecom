using System;
using System.Collections.Generic;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;

namespace Sofco.Model.Models.Rrhh
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
    }
}
