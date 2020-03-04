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

                Reason = contact?.Reason?.Text;
            }

        }

        public bool ApplicantInProgress { get; set; }

        public string Reason { get; set; }

        public int Id { get; set; }

        public string Applicant { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public DateTime? Date { get; set; }
    }

    public class ApplicantRelatedModel
    {
        public ApplicantRelatedModel(JobSearchApplicant jobSearchApplicant)
        {
            Id = jobSearchApplicant.Applicant.Id;

            Applicant = $"{jobSearchApplicant.Applicant.FirstName} {jobSearchApplicant.Applicant.LastName}";

            if (jobSearchApplicant.Applicant.ApplicantProfiles != null && jobSearchApplicant.Applicant.ApplicantProfiles.Any())
                Profiles = string.Join(";", jobSearchApplicant.Applicant.ApplicantProfiles.Select(x => x.Profile.Text));

            if (jobSearchApplicant.Applicant.ApplicantSkills != null && jobSearchApplicant.Applicant.ApplicantSkills.Any())
                Skills = string.Join(";", jobSearchApplicant.Applicant.ApplicantSkills.Select(x => x.Skill.Text));

            Date = jobSearchApplicant?.CreatedDate;

            Reason = jobSearchApplicant?.Reason?.Text;

            Status = jobSearchApplicant.Applicant.Status;
        }

        public ApplicantStatus Status { get; set; }

        public string Reason { get; set; }

        public int Id { get; set; }

        public string Applicant { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public DateTime? Date { get; set; }
    }
}
