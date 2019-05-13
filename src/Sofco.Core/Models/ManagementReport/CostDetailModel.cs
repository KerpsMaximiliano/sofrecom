using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class CostDetailModel
    {
        public string ManagerId { get; set; }
        public int ManagementReportId { get; set; }
        public int AnalyticId { get; set; }
        public IList<MonthDetailCost> MonthsHeader { get; set; }
        public List<CostResourceEmployee> CostEmployees { get; set; }
        public List<CostResource> FundedResources { get; set; }
        public List<CostResource> OtherResources { get; set; }
    }

    public class MonthDetailCost
    {
        public string Display { get; set; }
        public decimal? Value { get; set; }
        public decimal? OriginalValue { get; set; }
        public decimal? Adjustment { get; set; }
        public decimal? Charges { get; set; }
        public DateTime MonthYear { get; set; }
        public int CostDetailId { get; set; }
        public bool HasAlocation { get; set; }
        public decimal ValueEvalProp { get; set; }
        public int BillingMonthId { get; set; }
    }
    
    public class CostResource
    {
       // public int? EmployeeId { get; set; }
        //public int? UserId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string Display { get; set; }
        public bool show { get; set; }
        public bool OtherResource { get; set; }
        public string Description { get; set; }

        public IList<MonthDetailCost> MonthsCost { get; set; }
    }

    public class CostResourceEmployee
    {
        public int EmployeeId { get; set; }
        public int? UserId { get; set; }
        public string TypeName { get; set; }
        public string Display { get; set; }

        public IList<MonthDetailCost> MonthsCost { get; set; }
    }

}
