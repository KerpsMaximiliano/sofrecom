using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Utils
{
    public class ImputationNumber : Option
    {
        public IList<Solfac> Solfacs { get; set; }

        public ICollection<Analytic> Analytics { get; set; }
    }
}
