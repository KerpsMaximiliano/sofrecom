using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Relationships;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantAddModel
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string Comments { get; set; }

        public int? ClientId { get; set; }

        public int? ReasonCauseId { get; set; }

        public int? RecommendedByUserId { get; set; }

        public string CountryCode1 { get; set; }

        public string AreaCode1 { get; set; }

        public string Telephone1 { get; set; }

        public string CountryCode2 { get; set; }

        public string AreaCode2 { get; set; }

        public string Telephone2 { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Profiles { get; set; }

        public string ClientCrmId { get; set; }

        public Applicant CreateDomain()
        {
            var domain = new Applicant();

            SetData(domain);

            return domain;
        }

        private void SetData(Applicant domain)
        {
            domain.FirstName = FirstName;
            domain.LastName = LastName;
            domain.Email = Email;
            domain.Comments = Comments;
            domain.CountryCode1 = CountryCode1;
            domain.AreaCode1 = AreaCode1;
            domain.Telephone1 = Telephone1;
            domain.CountryCode2 = CountryCode2;
            domain.AreaCode2 = AreaCode2;
            domain.Telephone2 = Telephone2;

            if (ClientId.HasValue && ClientId > 0) domain.ClientId = ClientId.Value;
            if (RecommendedByUserId.HasValue && RecommendedByUserId > 0) domain.RecommendedByUserId = RecommendedByUserId.Value;

            if (Skills != null && Skills.Any())
            {
                domain.ApplicantSkills = Skills.Select(x => new ApplicantSkills { SkillId = x }).ToList();
            }

            if (Profiles != null && Profiles.Any())
            {
                domain.ApplicantProfiles = Profiles.Select(x => new ApplicantProfile() { ProfileId = x }).ToList();
            }
        }

        public void UpdateDomain(Applicant applicant)
        {
            SetData(applicant);

            if (Skills == null || !Skills.Any())
            {
                applicant.ApplicantSkills = new List<ApplicantSkills>();
            }

            if (Profiles == null || !Profiles.Any())
            {
                applicant.ApplicantProfiles = new List<ApplicantProfile>();
            }
        }
    }
}
