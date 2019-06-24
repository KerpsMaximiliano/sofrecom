using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.AdvancementAndRefund.Advancement
{
    public class SalaryAdvancementModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PendingAmount => TotalAmount - DiscountedAmount;

        public decimal DiscountedAmount { get; set; }

        public IList<SalaryAdvancementModelItem> Advancements { get; set; }
    }

    public class SalaryAdvancementModelItem
    {
        public int AdvancementId { get; set; }

        public string ReturnForm { get; set; }

        public IList<SalaryDiscountModel> Discounts { get; set; }
    }

    public class SalaryDiscountModel
    {
        public DateTime Date { get; set; }

        public decimal Amount { get; set; }
    }
}
