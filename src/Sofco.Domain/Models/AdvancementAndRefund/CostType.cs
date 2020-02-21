using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class CostType : Option
    {
        public int Category { get; set; }

        public IList<RefundDetail> RefundDetails { get; set; }
    }
}
