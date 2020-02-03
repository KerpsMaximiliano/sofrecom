using System;
using Sofco.Domain;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class UnemployeeListItemModel : BaseEntity
    {
        public UnemployeeListItemModel(Employee domain)
        {
            Id = domain.Id;
            EmployeeNumber = domain.EmployeeNumber;
            Name = domain.Name;
            StartDate = domain.StartDate;
            EndDate = domain.EndDate.GetValueOrDefault();
            EndReasonComments = domain.EndReason;

            if (domain.TypeEndReason != null)
                EndReasonType = domain.TypeEndReason.Text;
        }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string EndReasonType { get; set; }

        public string EndReasonComments { get; set; }
    }

    public class ReportUpdownItemModel
    {
        public ReportUpdownItemModel(Employee employee)
        {
            Name = employee.Name;
            EmployeeNumber = employee.EmployeeNumber;
            Profile = employee.Profile;
            StartDate = employee.StartDate;
            EndDate = employee.EndDate;
            Reason = employee.EndReason;
            State = EndDate.HasValue ? "BAJA" : "ALTA";
        }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public string Profile { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Reason { get; set; }

        public string State { get; set; }
    }
}
