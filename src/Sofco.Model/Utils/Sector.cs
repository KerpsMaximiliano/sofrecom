using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Rrhh;

namespace Sofco.Model.Utils
{
    public class Sector : Option
    {
        public ICollection<License> Licenses { get; set; }
        public ICollection<Analytic> Analytics { get; set; }
    }
}
