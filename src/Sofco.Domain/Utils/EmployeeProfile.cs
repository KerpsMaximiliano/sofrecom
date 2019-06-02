using System.Collections.Generic;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.Domain.Utils
{
    public class EmployeeProfile : Option
    {
        public ICollection<CostDetailProfile> CostDetailProfiles { get; set; }
    }
}
