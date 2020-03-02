using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantResultModel
    {
        public ApplicantResultModel(Applicant domain)
        {
            Id = domain.Id;
            FirstName = domain.FirstName;
            LastName = domain.LastName;
            Email = domain.Email;
            Status = domain.Status;

            if (domain.ApplicantProfiles != null && domain.ApplicantProfiles.Any())
                Profiles = string.Join(";", domain.ApplicantProfiles.Select(x => x.Profile.Text));

            if (domain.ApplicantSkills != null && domain.ApplicantSkills.Any())
                Skills = string.Join(";", domain.ApplicantSkills.Select(x => x.Skill.Text));
        }

        public ApplicantStatus Status { get; set; }

        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Skills { get; set; }
        public string Profiles { get; set; }
        public string Email { get; set; }
    }
}
