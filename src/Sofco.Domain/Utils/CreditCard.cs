using System.Collections.Generic;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Domain.Utils
{
    public class CreditCard : Option
    {
        public IList<Refund> Refunds { get; set; }

        public bool Active { get; set; }
    }
}
