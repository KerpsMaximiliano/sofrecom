﻿using System;
using Sofco.Domain;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class HoursApprovedModel : BaseEntity
    {
        public HoursApprovedModel(WorkTime domain)
        {
            if (domain.Employee != null)
            {
                Employee = $"{domain.Employee.EmployeeNumber}-{domain.Employee.Name}";
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

        public string Analytic { get; set; }

        public string Employee { get; set; }

        public string Task { get; set; }

        public DateTime Date { get; set; }

        public decimal Hours { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }
    }
}
