using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class SearchParams
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Status { get; set; }

        public int? ManagerId { get; set; }

        public string ClientId { get; set; }

        public int? AnalyticId { get; set; }

        public int? EmployeeId { get; set; }
    }
}
