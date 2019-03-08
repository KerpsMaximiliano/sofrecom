using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class AdvancementRefund
    {
        public int AdvancementId { get; set; }

        public Advancement Advancement { get; set; }

        public int RefundId { get; set; }

        public Refund Refund { get; set; }
    }
}
