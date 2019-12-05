using System;
using System.Collections.Generic;

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
        public bool IsReal { get; set; }
        public bool HasCostProfile { get; set; }
        public int Id { get; set; }
        public IList<DateTime> MonthsToReplicate { get; set; }
    }

    public class CostDetailStaffMonthModel
    {
        public int AnalyticId { get; set; }
        public int ManagementReportId { get; set; }
        public DateTime MonthYear { get; set; }
        public List<CostMonthEmployeeStaff> Employees { get; set; }
        public List<CostSubcategoryMonth> Subcategories { get; set; }
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

    public class MonthOther
    {
        public List<CostSubcategoryMonth> Subcategories { get; set; }
        public List<CostMonthOther> CostMonthOther { get; set; }
    }
    //public class CostSubtype
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class CostMonthOther
    {
        public int CostDetailId { get; set; }
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public string CategoryName { get; set; }
        public int CurrencyId { get; set; }
        public int Id { get; set; }
        public decimal? Value { get; set; }
        public string Description { get; set; }
    }

    public class CostSubcategoryMonth
    {
        public int Id { get; set; }
        public int CostDetailStaffId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Value { get; set; }
        public int IdCategory { get; set; }
        public string NameCategory { get; set; }
        public int BudgetTypeId { get; set; }
        public int CurrencyId { get; set; }
        public bool Deleted { get; set; }
    }
}
