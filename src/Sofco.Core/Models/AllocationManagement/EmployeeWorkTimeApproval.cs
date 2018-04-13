using System;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeWorkTimeApproval
    {
        public string EmployeeId { get; set; }

        public string Name { get; set; }

        public string Client { get; set; }

        public string ClientId { get; set; }

        public string Service { get; set; }

        public string ServiceId { get; set; }

        public int? ManagerId { get; set; }

        public string Manager { get; set; }

        public string ApprovalName { get; set; }

        public WorkTimeApproval WorkTimeApproval { get; set; }
    }
}
