using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Model.Utils
{
    public class ClientGroup : Option
    {
        public ICollection<Analytic> Analytics { get; set; }
    }
}
