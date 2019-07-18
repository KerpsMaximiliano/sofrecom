namespace Sofco.Core.Models.AdvancementAndRefund.Advancement
{
    public class SalaryAdvancementModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PendingAmount => TotalAmount - DiscountedAmount;

        public decimal DiscountedAmount { get; set; }

        public string Email { get; set; }
    }
}
