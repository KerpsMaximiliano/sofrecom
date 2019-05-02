using System;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ManagementReport : BaseEntity
    {
        public int AnalyticId { get; set; }

        public Analytic Analytic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
