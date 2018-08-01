using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.AllocationManagement
{
    public class AllocationMassiveAddModel
    {
        public IList<int> EmployeeIds { get; set; }

        public int AnalyticId { get; set; }

        public decimal? Percentage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
