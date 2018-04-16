using System;
using Sofco.Model;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeModel : BaseEntity
    {
        public WorkTimeModel(WorkTime domain)
        {
            Id = domain.Id;
            ServiceId = domain.ServiceId;
            Date = domain.Date;
            Hours = domain.Hours;
            Source = domain.Source;
            UserComment = domain.UserComment;
            ApprovalComment = domain.ApprovalComment;

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
        }

        public string ServiceId { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public int TaskId { get; set; }
        public string TaskName { get; set; }

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public string Source { get; set; }

        public string UserComment { get; set; }

        public string ApprovalComment { get; set; }
    }
}
