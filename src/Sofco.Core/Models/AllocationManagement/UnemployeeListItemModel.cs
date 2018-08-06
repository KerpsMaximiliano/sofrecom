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
}
