using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlResourceModel
    {
        public string Id { get; set; }

        public string Analytic { get; set; }

        public string EmployeeNumber { get; set; }

        public string EmployeeName { get; set; }

        public decimal BusinessHours { get; set; }

        public decimal ApprovedHours { get; set; }

        public decimal LicenseHours { get; set; }

        public decimal SentHours { get; set; }

        public decimal DraftHours { get; set; }

        public decimal AllocationPercentage { get; set; }

        public List<WorkTimeControlResourceDetailModel> Details { get; set; }

        public decimal PendingHours { get; set; }

        public int DetailCount { get; set; }
    }
}
