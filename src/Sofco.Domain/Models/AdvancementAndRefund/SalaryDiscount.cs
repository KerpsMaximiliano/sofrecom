using System;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class SalaryDiscount : BaseEntity
    {
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
