using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class SalaryDiscount : BaseEntity
    {
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public int AdvancementId { get; set; }

        public Advancement Advancement { get; set; }
    }
}
