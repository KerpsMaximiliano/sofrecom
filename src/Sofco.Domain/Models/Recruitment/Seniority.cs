using System.Collections.Generic;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class Seniority : Option
    {
        public IList<ResourceBilling> ResourceBillings { get; set; }

        public IList<JobSearchSeniority> JobSearchSeniorities { get; set; }
        public IList<Applicant> Applicants { get; set; }
    }
}
