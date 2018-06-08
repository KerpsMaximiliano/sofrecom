using System;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class ResourceForAnalyticsModel
    {
        public ResourceForAnalyticsModel(Allocation allocation)
        {
            if (allocation.Employee != null)
            {
                Resource = $"{allocation.Employee.EmployeeNumber} - {allocation.Employee.Name}";
            }

            StartDate = allocation.StartDate;
            ReleaseDate = allocation.ReleaseDate;
            Percentage = allocation.Percentage;

            EndDate = allocation.StartDate.AddMonths(1).AddDays(-1);

            EmployeeId = allocation.EmployeeId;
        }

        public int EmployeeId { get; set; }

        public DateTime EndDate { get; set; }

        public string Resource { get; set; }

        public DateTime ReleaseDate { get; set; }

        public DateTime StartDate { get; set; }

        public decimal Percentage { get; set; }
    }
}
