﻿namespace Sofco.Domain.Models.WorkTimeManagement
{
    public class WorkTimeApprovalEmployee
    {
        public string EmployeeId { get; set; }

        public string Name { get; set; }

        public string Client { get; set; }

        public string ClientId { get; set; }

        public string Service { get; set; }

        public int? AnalyticId { get; set; }

        public string Analytic { get; set; }

        public int? ManagerId { get; set; }

        public string Manager { get; set; }

        public string ApprovalName { get; set; }

        public WorkTimeApproval WorkTimeApproval { get; set; }
    }
}
