using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class PaymentTerm : Option
    {
        public IList<Solfac> Solfacs { get; set; }
    }
}
