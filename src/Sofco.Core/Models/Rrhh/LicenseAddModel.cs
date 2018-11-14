using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.Models.Rrhh
{
    public class LicenseAddModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int ManagerId { get; set; }

        public string ManagerDesc { get; set; }

        public int SectorId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int TypeId { get; set; }

        public bool WithPayment { get; set; }

        public int DaysQuantity { get; set; }

        public bool HasCertificate { get; set; }

        public bool Parcial { get; set; }

        public bool Final { get; set; }

        public string Comments { get; set; }

        public string ExamDescription { get; set; }

        public int UserId { get; set; }

        public int EmployeeLoggedId { get; set; }

        public bool IsRrhh { get; set; }

        public License CreateDomain()
        {
            var domain = new License();

            domain.EmployeeId = EmployeeId;
            domain.ManagerId = ManagerId;
            domain.SectorId = SectorId;
            domain.StartDate = StartDate.GetValueOrDefault().Date;
            domain.EndDate = EndDate.GetValueOrDefault().Date;
            domain.TypeId = TypeId;
            domain.WithPayment = WithPayment;
            domain.DaysQuantity = DaysQuantity;
            domain.HasCertificate = HasCertificate;
            domain.Parcial = Parcial;
            domain.Final = Final;
            domain.Comments = Comments;
            domain.ExamDescription = ExamDescription;
            domain.CreationDate = DateTime.UtcNow;
            domain.Status = LicenseStatus.Draft;

            return domain;
        }
    }
}
