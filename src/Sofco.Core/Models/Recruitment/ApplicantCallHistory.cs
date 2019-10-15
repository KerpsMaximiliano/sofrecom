using System;
using System.Linq;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantCallHistory
    {
        public ApplicantCallHistory(JobSearchApplicant domain)
        {
            Date = domain.CreatedDate;
            CreatedBy = domain.CreatedBy;
            Comments = domain.Comments;
            JobSearchId = domain.JobSearchId;

            if (domain.Reason != null) Reason = domain.Reason.Text;
            if (domain.JobSearch.Client != null) Client = domain.JobSearch.Client.Name;

            if (domain.JobSearch.JobSearchProfiles != null && domain.JobSearch.JobSearchProfiles.Any())
                Profiles = string.Join(";", domain.JobSearch.JobSearchProfiles.Select(x => x.Profile.Text));

            if (domain.JobSearch.JobSearchSkillsRequired != null && domain.JobSearch.JobSearchSkillsRequired.Any())
                Skills = string.Join(";", domain.JobSearch.JobSearchSkillsRequired.Select(x => x.Skill.Text));
        }

        public DateTime Date { get; set; }

        public string Reason { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public string Client { get; set; }

        public int JobSearchId { get; set; }
    }
}
