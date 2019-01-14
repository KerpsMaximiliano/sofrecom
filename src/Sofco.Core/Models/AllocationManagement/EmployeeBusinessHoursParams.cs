namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeBusinessHoursParams
    {
        public int? BusinessHours { get; set; }

        public string BusinessHoursDescription { get; set; }

        public string Office { get; set; }

        public int? HolidaysPending { get; set; }

        public int? ManagerId { get; set; }

        public decimal? BillingPercentage { get; set; }

        public bool HasCreditCard { get; set; }
    }
}
