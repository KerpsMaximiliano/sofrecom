using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class ResourceAssignment : Option
    {
        public IList<JobSearch> JobSearchs { get; set; }
    }
}
