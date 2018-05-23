﻿using System;
using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeProfileModel
    {
        public EmployeeProfileModel()
        {
            Allocations = new List<EmployeeAllocationModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string EmployeeNumber { get; set; }

        public string Seniority { get; set; }

        public string Profile { get; set; }

        public string Technology { get; set; }

        public decimal Percentage { get; set; }

        public string OfficeAddress { get; set; }

        public string Manager { get; set; } 

        public IList<EmployeeAllocationModel> Allocations { get; set; }

        public IList<EmployeeHistoryModel> History { get; set; }

        public string Address { get; set; }

        public string Location { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public HealthInsurance HealthInsurance { get; set; }

        public PrepaidHealth PrepaidHealth { get; set; }

        public int HolidaysPending { get; set; }

        public int HolidaysPendingByLaw { get; set; }

        public int HolidaysByLaw { get; set; }

        public int ExtraHolidaysQuantity { get; set; }

        public int ExtraHolidaysQuantityByLaw { get; set; }

        public bool HasExtraHolidays { get; set; }

        public DateTime StartDate { get; set; }

        public int BusinessHours { get; set; }

        public string BusinessHoursDescription { get; set; }
    }
}
