using System;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchApplicantModel
    {
        public JobSearchApplicantModel(Applicant applicant)
        {
            Id = applicant.Id;
            Applicant = $"{applicant.FirstName} {applicant.LastName}";

            if (applicant.ApplicantProfiles != null && applicant.ApplicantProfiles.Any())
                Profiles = string.Join(";", applicant.ApplicantProfiles.Select(x => x.Profile.Text));

            if (applicant.ApplicantSkills != null && applicant.ApplicantSkills.Any())
                Skills = string.Join(";", applicant.ApplicantSkills.Select(x => x.Skill.Text));

            if (applicant.JobSearchApplicants != null && applicant.JobSearchApplicants.Any())
            {
                var contact = applicant.JobSearchApplicants.FirstOrDefault();
                Date = contact?.CreatedDate;

                ApplicantInProgress = applicant.JobSearchApplicants.Any(x => x.Reason.Type == ReasonCauseType.ApplicantInProgress);
            }
        }

        public bool ApplicantInProgress { get; set; }

        public int Id { get; set; }

        public string Applicant { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public DateTime? Date { get; set; }
    }
}
