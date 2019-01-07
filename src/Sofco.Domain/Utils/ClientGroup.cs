using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Utils
{
    public class ClientGroup : Option
    {
        public ICollection<Analytic> Analytics { get; set; }
    }
}
