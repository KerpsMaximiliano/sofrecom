using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Relationships;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchAddModel
    {
        public int? UserId { get; set; }

        public int? RecruiterId { get; set; }

        public int? ReasonCauseId { get; set; }

        public string ClientCrmId { get; set; }

        public int ClientId { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Seniorities { get; set; }

        public int? Quantity { get; set; }

        public string TimeHiring { get; set; }

        public decimal? MaximunSalary { get; set; }

        public string Comments { get; set; }

        public JobSearch CreateDomain()
        {
            var domain = new JobSearch();

            domain.ClientId = ClientId;
            domain.ReasonCauseId = ReasonCauseId.GetValueOrDefault();
            domain.UserId = UserId.GetValueOrDefault();
            domain.Comments = Comments;
            domain.MaximunSalary = MaximunSalary.GetValueOrDefault();
            domain.Quantity = Quantity.GetValueOrDefault();
            domain.TimeHiring = TimeHiring;
            domain.RecruiterId = RecruiterId.GetValueOrDefault();

            domain.Status = JobSearchStatus.Open;

            if (Profiles != null && Profiles.Any())
            {
                domain.JobSearchProfiles = Profiles.Select(x => new JobSearchProfile { ProfileId = x }).ToList();
            }

            if (Seniorities != null && Seniorities.Any())
            {
                domain.JobSearchSeniorities = Seniorities.Select(x => new JobSearchSeniority { SeniorityId = x }).ToList();
            }

            if (Skills != null && Skills.Any())
            {
                domain.JobSearchSkills = Skills.Select(x => new JobSearchSkill { SkillId = x }).ToList();
            }

            return domain;
        }
    }
}
