using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailResource : BaseEntity
    {
        public int CostDetailId { get; set; }
        public CostDetail CostDetail { get; set; }

        public string Value { get; set; }
        public decimal? Adjustment { get; set; }
        public string Charges { get; set; }
        //public bool IsReal { get; set; }
        public int BudgetTypeId { get; set; }
        public BudgetType BudgetType { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public string Comments { get; set; }

        public decimal? PercentageModified { get; set; }

        public string Bono { get; set; }
    }
}
