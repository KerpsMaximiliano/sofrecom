using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AllocationManagement
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

        public string CreatedByUser { get; set; }

        public string Address { get; set; }

        public string Location { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public int HealthInsuranceCode { get; set; }

        public int PrepaidHealthCode { get; set; }

        public string OfficeAddress { get; set; }

        public int ExtraHolidaysQuantity { get; set; }

        public int ExtraHolidaysQuantityByLaw { get; set; }

        public bool HasExtraHolidays { get; set; }

        public int HolidaysPending { get; set; }

        public int ExamDaysTaken { get; set; }

        public ICollection<License> Licenses { get; set; }

        public string Email { get; set; }

        public int HolidaysByLaw { get; set; }

        public int HolidaysPendingByLaw { get; set; }

        public int BusinessHours { get; set; }

        public string BusinessHoursDescription { get; set; }

        public ICollection<EmployeeCategory> EmployeeCategories { get; set; }

        public ICollection<WorkTime> WorkTimes { get; set; }

        public string EndReason { get; set; }

        public int? TypeEndReasonId { get; set; }

        public EmployeeEndReason TypeEndReason { get; set; }

        public int? ManagerId { get; set; }

        public User Manager { get; set; }

        public string DocumentNumberType { get; set; }

        public int DocumentNumber { get; set; }

        public decimal Cuil { get; set; }

        public int PhoneCountryCode { get; set; }

        public int PhoneAreaCode { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsExternal { get; set; }

        public bool HasCreditCard { get; set; }

        public string Bank { get; set; }

        public decimal Salary { get; set; }

        public int BeneficiariesCount { get; set; }

        public decimal PrepaidAmount { get; set; }

        public string PrepaidPlan { get; set; }

        public ICollection<CostDetailResource> CostDetailResources { get; set; }
    }
}
