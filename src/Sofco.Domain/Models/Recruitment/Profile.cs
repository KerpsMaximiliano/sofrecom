using System.Collections.Generic;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class Profile : Option
    {
        public IList<ResourceBilling> ResourceBillings { get; set; }

        public IList<JobSearchProfile> JobSearchProfiles { get; set; }

        public IList<ApplicantProfile> ApplicantProfiles { get; set; }

        public IList<Applicant> Applicants { get; set; }
    }
}
