using System.Collections.Generic;
using Sofco.Model.Models.Billing;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.Model.Utils
{
    public class Currency : Option
    {
        public IList<Solfac> Solfacs { get; set; }
        public ICollection<Analytic> Analytics { get; set; }
    }
}
