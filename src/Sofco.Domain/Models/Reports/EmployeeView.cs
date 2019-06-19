using System;

namespace Sofco.Domain.Models.Reports
{
    public class EmployeeView
    {
        public string EmployeeNumber { get; set; }

        public string Name { get; set; }

        public string Profile { get; set; }

        public string Seniority { get; set; }

        public string Technology { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Email { get; set; }

        public decimal Cuil { get; set; }

        public int DocumentNumber { get; set; }

        public DateTime Birthday { get; set; }

        public string OfficeAddress { get; set; }

        public string Address { get; set; }

        public string Location { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public int PhoneAreaCode { get; set; }

        public int PhoneCountryCode { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// VACACIONES_POR_LEY
        /// </summary>
        public int HolidaysByLaw { get; set; }

        /// <summary>
        /// VACACIONES_PENDIENTES_POR_LEY
        /// </summary>
        public int HolidaysPendingByLaw { get; set; }

        /// <summary>
        /// VACACIONES_EXTRAS
        /// </summary>
        public int ExtraHolidaysQuantity { get; set; }

        public bool HasExtraHolidays { get; set; }

        /// <summary>
        /// VACACIONES_PENDIENTES
        /// </summary>
        public int HolidaysPending { get; set; }

        public int ExamDaysTaken { get; set; }

        public decimal BillingPercentage { get; set; }

        public int BusinessHours { get; set; }

        public string BusinessHoursDescription { get; set; }

        public int PrepaidHealthCode { get; set; }

        public string EndReason { get; set; }

        public bool IsExternal { get; set; }

        public bool HasCreditCard { get; set; }

        public string Bank { get; set; }

        public decimal Percentage { get; set; }

        public string Title { get; set; }

        public string AnalyticName { get; set; }
        
        public string ServiceName { get; set; }

        public string ManagerName { get; set; }

        public int Id { get; set; }
    }
}
