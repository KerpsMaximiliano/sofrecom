using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantDetailModel
    {
        public ApplicantDetailModel(Applicant applicant)
        {
            LastName = applicant.LastName;
            FirstName = applicant.FirstName;
            Email = applicant.Email;
            Comments = applicant.Comments;
            ReasonCauseId = applicant.ReasonCauseId;
            CountryCode1 = applicant.CountryCode1;
            AreaCode1 = applicant.AreaCode1;
            Telephone1 = applicant.Telephone1;
            CountryCode2 = applicant.CountryCode2;
            AreaCode2 = applicant.AreaCode2;
            Telephone2 = applicant.Telephone2;

            if (applicant.RecommendedByUserId.HasValue) RecommendedByUserId = applicant.RecommendedByUserId.Value.ToString();

            if (applicant.Client != null) ClientId = applicant.Client.CrmId;

            if (applicant.ApplicantProfiles != null && applicant.ApplicantProfiles.Any())
                Profiles = applicant.ApplicantProfiles.Select(x => x.ProfileId).ToList();

            if (applicant.ApplicantSkills != null && applicant.ApplicantSkills.Any())
                Skills = applicant.ApplicantSkills.Select(x => x.SkillId).ToList();
        }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string Comments { get; set; }

        public int? ReasonCauseId { get; set; }

        public string RecommendedByUserId { get; set; }

        public string CountryCode1 { get; set; }

        public string AreaCode1 { get; set; }

        public string Telephone1 { get; set; }

        public string CountryCode2 { get; set; }

        public string AreaCode2 { get; set; }

        public string Telephone2 { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Profiles { get; set; }

        public string ClientId { get; set; }
    }
}
