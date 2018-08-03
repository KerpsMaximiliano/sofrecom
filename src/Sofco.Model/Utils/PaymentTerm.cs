using System.Collections.Generic;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Utils
{
    public class PaymentTerm : Option
    {
        public IList<Solfac> Solfacs { get; set; }
    }
}
