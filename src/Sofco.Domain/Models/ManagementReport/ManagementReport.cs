using System;
using System.Collections;
using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ManagementReport : BaseEntity
    {
        public int AnalyticId { get; set; }

        public Analytic Analytic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<CostDetail> CostDetails { get; set; }

        public ICollection<ManagementReportBilling> Billings { get; set; }
    }
}
