using System.Collections.Generic;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class Skill : Option
    {
        public IList<JobSearchSkillRequired> JobSearchSkillsRequired { get; set; }
        public IList<JobSearchSkillNotRequired> JobSearchSkillsNotRequired { get; set; }
        public IList<ApplicantSkills> ApplicantProfiles { get; set; }
    }
}
