using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.WorkTimeManagement
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

        public decimal Hours { get; set; }

        public string Source { get; set; }

        public WorkTimeStatus Status { get; set; }

        public int? ApprovalUserId { get; set; }

        public string UserComment { get; set; }

        public string ApprovalComment { get; set; }

        public DateTime CreationDate { get; set; }

        public string Reference { get; set; }
    }
}
