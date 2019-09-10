using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchModel
    {
        public JobSearchModel(JobSearch jobsearch)
        {
            UserId = jobsearch.UserId;
            RecruiterId = jobsearch.RecruiterId;
            ReasonCauseId = jobsearch.ReasonCauseId;
            Quantity = jobsearch.Quantity;
            TimeHiring = jobsearch.TimeHiring;
            MaximunSalary = jobsearch.MaximunSalary;
            Comments = jobsearch.Comments;

            if (jobsearch.Client != null) ClientCrmId = jobsearch.Client.CrmId;

            if (jobsearch.JobSearchProfiles != null && jobsearch.JobSearchProfiles.Any())
                Profiles = jobsearch.JobSearchProfiles.Select(x => x.ProfileId).ToList();

            if (jobsearch.JobSearchSeniorities != null && jobsearch.JobSearchSeniorities.Any())
                Seniorities = jobsearch.JobSearchSeniorities.Select(x => x.SeniorityId).ToList();

            if (jobsearch.JobSearchSkills != null && jobsearch.JobSearchSkills.Any())
                Skills = jobsearch.JobSearchSkills.Select(x => x.SkillId).ToList();
        }

        public int UserId { get; set; }

        public int RecruiterId { get; set; }

        public int ReasonCauseId { get; set; }

        public string ClientCrmId { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Seniorities { get; set; }

        public int Quantity { get; set; }

        public string TimeHiring { get; set; }

        public decimal MaximunSalary { get; set; }

        public string Comments { get; set; }
    }
}
