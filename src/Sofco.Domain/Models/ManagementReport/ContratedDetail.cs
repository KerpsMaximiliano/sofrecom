using Sofco.Domain.Models.AllocationManagement;
using System;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ContratedDetail : BaseEntity
    {
        public int IdAnalytic { get; set; }
        public Analytic Analytic { get; set; }

        public string Name { get; set; }
        public DateTime MonthYear { get; set; }
        public float? insurance { get; set; }
        public float? honorary { get; set; }

    }
}
