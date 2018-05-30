using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class Currency : Option
    {
        public IList<Solfac> Solfacs { get; set; }

        public string CrmProductionId { get; set; }

        public string CrmDevelopmentId { get; set; }
    }
}
