using System;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeAddModel : BaseEntity
    {
        public int AnalyticId { get; set; }

        public int EmployeeId { get; set; }

        public int UserId { get; set; }

        public int TaskId { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string Source { get; set; }

        public string UserComment { get; set; }

        public string ApprovalComment { get; set; }

        public string Reference { get; set; }

        public WorkTimeStatus Status { get; set; }

        public WorkTime CreateDomain()
        {
            var domain = new WorkTime();

            domain.Id = Id;
            domain.AnalyticId = AnalyticId;
            domain.EmployeeId = EmployeeId;
            domain.UserId = UserId;
            domain.TaskId = TaskId;
            domain.Date = Date;
            domain.Hours = Hours;
            domain.Source = WorkTimeSource.Manual.ToString();
            domain.UserComment = UserComment;
            domain.Reference = Reference;

            domain.CreationDate = DateTime.UtcNow.Date;
            domain.Status = WorkTimeStatus.Draft;

            return domain;
        }
    }
}
