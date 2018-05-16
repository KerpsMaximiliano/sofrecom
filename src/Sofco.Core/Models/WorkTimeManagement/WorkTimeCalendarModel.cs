using System;
using Sofco.Model;
using Sofco.Model.Enums;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeCalendarModel : BaseEntity
    {
        public WorkTimeCalendarModel(WorkTime domain)
        {
            Id = domain.Id;
            Date = domain.Date;
            Hours = domain.Hours;
            Source = domain.Source;
            UserComment = domain.UserComment;
            ApprovalComment = domain.ApprovalComment;
            Status = domain.Status;

            if (domain.Employee != null)
            {
                EmployeeId = domain.EmployeeId;
                EmployeeName = domain.Employee.Name;
            }

            if (domain.Task != null)
            {
                TaskId = domain.TaskId;
                TaskName = domain.Task.Description;
            }

            if (domain.Analytic != null)
            {
                AnalyticId = domain.AnalyticId;
                AnalyticName = domain.Analytic.Name;
            }
        }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public int TaskId { get; set; }
        public string TaskName { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string Source { get; set; }

        public string UserComment { get; set; }

        public string ApprovalComment { get; set; }

        public int AnalyticId { get; set; }

        public string AnalyticName { get; set; }

        public WorkTimeStatus Status { get; set; }
    }
}
