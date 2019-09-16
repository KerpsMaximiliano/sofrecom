using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ManagementReport : BaseEntity
    {
        public int AnalyticId { get; set; }

        public Analytic Analytic { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ManagementReportStatus Status { get; set; }

        public BudgetType State { get; set; }

        public ICollection<CostDetail> CostDetails { get; set; }

        public ICollection<ManagementReportBilling> Billings { get; set; }

        public IList<Budget> Budgets { get; set; } 

        public IList<ManagementReportComment> Comments { get; set; } 
    }
}
