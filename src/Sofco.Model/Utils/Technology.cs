using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Model.Utils
{
    public class Technology : Option
    {
        public ICollection<Analytic> Analytics { get; set; }
    }
}
