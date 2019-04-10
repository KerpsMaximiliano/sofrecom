﻿using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class CostDetailModel
    {
        public string ManagerId { get; set; }
        public int AnalyticId { get; set; }
        public IList<MonthDetailCost> MonthsHeader { get; set; }
        public List<CostResource> CostEmployees { get; set; }
        public List<CostResource> FundedResources { get; set; }
    }

    public class MonthDetailCost
    {
        public string Display { get; set; }
        public float? Value { get; set; }
        public DateTime MonthYear { get; set; }
        public int CostDetailId { get; set; }
    }
    
    public class CostResource
    {
        public int? EmployeeId { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string Display { get; set; }

        public IList<MonthDetailCost> MonthsCost { get; set; }
    }

}
