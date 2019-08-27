using System.Collections.Generic;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class Seniority : Option
    {
        public IList<ResourceBilling> ResourceBillings { get; set; }
    }
}
