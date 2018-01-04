using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Model.Utils
{
    public class ServiceType : Option
    {
        public ICollection<Analytic> Analytics { get; set; }
    }
}
