using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Model.Utils
{
    public class Solution : Option
    {
        public ICollection<Analytic> Analytics { get; set; }
    }
}
