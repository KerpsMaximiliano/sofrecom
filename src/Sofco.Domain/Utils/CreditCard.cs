using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Domain.Utils
{
    public class CreditCard : Option
    {
        public IList<Refund> Refunds { get; set; }
    }
}
