using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class CostDetailModel
    {
        public string ManagerId { get; set; }
        public IList<MonthDetailCost> MonthsHeader { get; set; }
        public IList<Employee> Employees { get; set; }

    }

    public class MonthDetailCost
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public string Display { get; set; }
    }
}
