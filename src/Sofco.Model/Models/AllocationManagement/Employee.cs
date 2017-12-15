﻿using System;
using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class Employee : BaseEntity, IEntityDate
    {
        public string EmployeeNumber { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public string Seniority { get; set; }

        public decimal BillingPercentage { get; set; }

        public ICollection<Allocation> Allocations { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
