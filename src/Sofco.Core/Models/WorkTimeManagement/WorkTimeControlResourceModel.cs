using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlResourceModel
    {
        public int Id { get; set; }

        public string Analytic { get; set; }

        public string EmployeeNumber { get; set; }

        public string EmployeeName { get; set; }

        public decimal BusinessHours { get; set; }

        public decimal RegisteredHours { get; set; }

        public decimal LicenseHours { get; set; }

        public decimal PendingHours { get; set; }

        public List<WorkTimeControlResourceDetailModel> Details { get; set; }
    }
}
