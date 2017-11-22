using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class ImputationNumber : Option
    {
        public IList<Solfac> Solfacs { get; set; }
        public ICollection<Analytic> Analytics { get; set; }
    }
}
