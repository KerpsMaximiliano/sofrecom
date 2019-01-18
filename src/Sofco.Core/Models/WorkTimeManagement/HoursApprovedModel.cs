﻿using System;
using System.Collections.Generic;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class HoursApprovedModel : BaseEntity
    {
        public HoursApprovedModel(WorkTime domain)
        {
            if (domain.Employee != null)
            {
                Employee = $"{domain.Employee.Name}";
                EmployeeNumber = $"{domain.Employee.EmployeeNumber}";
            }

            if (domain.Analytic != null)
            {
                Analytic = $"{domain.Analytic.Title}-{domain.Analytic.Name}";
            }

            if (domain.Task != null)
            {
                Task = domain.Task.Description;
            }

            Id = domain.Id;
            Date = domain.Date;
            Hours = domain.Hours;
            Comments = domain.UserComment;
            Status = domain.Status.ToString();
        }

        public string EmployeeNumber { get; set; }

        public string Analytic { get; set; }

        public string Employee { get; set; }

        public string Task { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public IList<BankHoursSplitted> HoursSplitteds { get; set; }
    }

    public class BankHoursSplitted
    {
        public int AnalyticId { get; set; }

        public int EmployeeId { get; set; }

        public int UserId { get; set; }

        public int TaskId { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string Source { get; set; }

        public WorkTimeStatus Status { get; set; }

        public string UserComment { get; set; }

        public DateTime CreationDate { get; set; }

        public string Analytic { get; set; }
    }

    public class HoursToApproveModel
    {
        public int Id { get; set; }

        public IList<BankHoursSplitted> HoursSplitteds { get; set; }
    }
}
