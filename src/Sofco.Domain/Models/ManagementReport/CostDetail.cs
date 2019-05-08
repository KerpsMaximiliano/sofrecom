using System;
using System.Collections.Generic;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetail : BaseEntity
    {
        public int ManagementReportId { get; set; }
        public ManagementReport ManagementReport { get; set; }

        public DateTime MonthYear { get; set; }

        public ICollection<CostDetailResource> CostDetailResources { get; set; }

        public ICollection<CostDetailProfile> CostDetailProfiles { get; set; }

        public ICollection<CostDetailOther> CostDetailOthers { get; set; }

        public ICollection<ContratedDetail> ContratedDetails { get; set; }
    }
}
