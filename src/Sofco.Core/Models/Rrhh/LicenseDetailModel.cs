using System;
using System.Collections.Generic;
using Sofco.Model;
using Sofco.Model.Enums;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Utils;

namespace Sofco.Core.Models.Rrhh
{
    public class LicenseDetailModel : BaseEntity
    {
        public LicenseDetailModel(License license)
        {
            Id = license.Id;
            StartDate = license.StartDate.Date;
            EndDate = license.EndDate.Date;
            WithPayment = license.WithPayment;
            DaysQuantity = license.DaysQuantityByLaw;
            HasCertificate = license.HasCertificate;
            Parcial = license.Parcial;
            Final = license.Final;
            Comments = license.Comments;
            ExamDescription = license.ExamDescription;
            Status = license.Status;
            StatusName = license.Status.ToString();
            CreationDate = license.CreationDate.Date;

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
                TypeId = license.TypeId;
                TypeName = license.Type.Description;
                CertificateRequired = license.Type.CertificateRequired;
            }

            if (license.Sector != null)
            {
                SectorId = license.Id;
                SectorName = license.Sector.Text;
            }

            Files = new List<Option>();

            foreach (var licenseFile in license.LicenseFiles)
            {
                if(licenseFile.File != null)
                    Files.Add(new Option { Id = licenseFile.FileId, Text = licenseFile.File.FileName });
            }
        }

        public bool CertificateRequired { get; set; }

        public string StatusName { get; set; }
        public DateTime CreationDate { get; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public int ManagerId { get; set; }
        public string ManagerName { get; set; }

        public int SectorId { get; set; }
        public string SectorName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TypeId { get; set; }
        public string TypeName { get; set; }

        public bool WithPayment { get; set; }

        public int DaysQuantity { get; set; }

        public bool HasCertificate { get; set; }

        public bool Parcial { get; set; }

        public bool Final { get; set; }

        public string Comments { get; set; }

        public string ExamDescription { get; set; }

        public LicenseStatus Status { get; set; }

        public IList<Option> Files { get; set; }
    }
}
