using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Relationships
{
    public class JobSearchSkill
    {
        public int JobSearchId { get; set; }
        public JobSearch JobSearch { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
