using System.Collections.Generic;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class Skill : Option
    {
        public IList<JobSearchSkill> JobSearchSkills { get; set; }
    }
}
