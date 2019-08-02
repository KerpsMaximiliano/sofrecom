using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class SearchParams
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Status { get; set; }

        public IList<int> ManagerId { get; set; }

        public IList<int> AnalyticId { get; set; }

        public int? EmployeeId { get; set; }
    }
}
