using System;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Model.Models.WorkTimeManagement
{
    public class WorkTime : BaseEntity
    {
        public int AnalyticId { get; set; }
        public Analytic Analytic { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int TaskId { get; set; }
        public Task Task { get; set; }

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public string Source { get; set; }

        public WorkTimeStatus Status { get; set; }

        public int ApprovalUserId { get; set; }
        public User ApprovalUser { get; set; }

        public string UserComment { get; set; }

        public string ApprovalComment { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
