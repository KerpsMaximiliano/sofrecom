using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Relationships
{
    public class ApplicantSkills
    {
        public int ApplicantId { get; set; }

        public Applicant Applicant { get; set; }

        public int SkillId { get; set; }

        public Skill Skill { get; set; }
    }
}
