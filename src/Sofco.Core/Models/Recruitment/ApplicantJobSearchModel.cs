using System.Linq;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantJobSearchModel
    {
        public ApplicantJobSearchModel(JobSearch jobSearch, int applicantId)
        {
            Id = jobSearch.Id;
            Client = jobSearch.Client?.Name;
            Reason = jobSearch.ReasonCause?.Text;

            if (jobSearch.JobSearchApplicants != null)
                ContactedBefore = jobSearch.JobSearchApplicants.Any(x => x.ApplicantId == applicantId);
           
            if (jobSearch.JobSearchProfiles != null && jobSearch.JobSearchProfiles.Any())
                Profiles = string.Join(";", jobSearch.JobSearchProfiles.Select(x => x.Profile.Text));

            if (jobSearch.JobSearchSkillsRequired != null && jobSearch.JobSearchSkillsRequired.Any())
                Skills = string.Join(";", jobSearch.JobSearchSkillsRequired.Select(x => x.Skill.Text));
        }

        public bool ContactedBefore { get; set; }

        public int Id { get; set; }

        public string Client { get; set; }

        public string Reason { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }
    }
}
