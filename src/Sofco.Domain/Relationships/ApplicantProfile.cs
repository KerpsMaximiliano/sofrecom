using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Relationships
{
    public class ApplicantProfile
    {
        public int ApplicantId { get; set; }

        public Applicant Applicant { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }
    }
}
