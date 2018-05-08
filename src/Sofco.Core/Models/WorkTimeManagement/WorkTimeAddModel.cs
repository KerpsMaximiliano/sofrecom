﻿using System;
using Sofco.Model;
using Sofco.Model.Enums;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeAddModel : BaseEntity
    {
        public int AnalyticId { get; set; }

        public int EmployeeId { get; set; }

        public int UserId { get; set; }

        public int TaskId { get; set; }

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public string Source { get; set; }

        public string UserComment { get; set; }

        public string ApprovalComment { get; set; }

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
            domain.Source = Source;
            domain.UserComment = UserComment;

            domain.CreationDate = DateTime.UtcNow;
            domain.Status = WorkTimeStatus.Draft;

            return domain;
        }
    }
}
