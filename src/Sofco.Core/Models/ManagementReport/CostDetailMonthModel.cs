using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class CostDetailMonthModel
    {
        public int AnalyticId { get; set; }
        public int ManagementReportId { get; set; }
        public DateTime MonthYear { get; set; }
        public List<CostMonthEmployee> Employees { get; set; }
        public List<CostMonthOther> OtherResources { get; set; }
        public List<ContractedModel> Contracted { get; set; }
        public decimal? Provision { get; set; }
        public decimal? TotalBilling { get; set; }
        public decimal? TotalProvisioned { get; set; }
        public int Id { get; set; }
    }

    public class CostDetailStaffMonthModel
    {
        public int AnalyticId { get; set; }
        public int ManagementReportId { get; set; }
        public DateTime MonthYear { get; set; }
        public List<CostMonthEmployeeStaff> Employees { get; set; }
        public List<CostMonthOther> OtherResources { get; set; }
        public List<ContractedModel> Contracted { get; set; }
        public decimal? TotalProvisioned { get; set; }
        public int Id { get; set; }
    }

    public class CostMonthEmployee
    {
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        public int Id { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Charges { get; set; }
    }

    public class CostMonthEmployeeStaff : CostMonthEmployee
    {
        public decimal Total { get; set; }

        public string Name { get; set; }

        public DateTime MonthYear { get; set; }

        public decimal ChargesPercentage { get; set; }
    }

    public class CostMonthOther
    {
        public int CostDetailId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int Id { get; set; }
        public decimal? Value { get; set; }
        public string Description { get; set; }
    }
}
