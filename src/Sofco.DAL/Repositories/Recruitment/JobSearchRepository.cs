using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Recruitment;
using Sofco.Core.Models.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Repositories.Recruitment
{
    public class JobSearchRepository : BaseRepository<JobSearch>, IJobSearchRepository
    {
        public JobSearchRepository(SofcoContext context) : base(context)
        {
        }
         
        public IList<JobSearch> Search(JobSearchParameter parameter)
        {
            var query = context.JobSearchs
                .Include(x => x.Recruiter)
                .Include(x => x.User)
                .Include(x => x.Client)
                .Include(x => x.ReasonCause)
                .Include(x => x.JobSearchSkillsRequired)
                    .ThenInclude(x => x.Skill)
                .Include(x => x.JobSearchProfiles)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.JobSearchSeniorities)
                    .ThenInclude(x => x.Seniority)
                .Where(x => parameter.Status.Contains(x.Status));

            if (parameter.Id.HasValue && parameter.Id.Value > 0)
            {
                query = query.Where(x => x.Id == parameter.Id);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(parameter.ClientId))
                    query = query.Where(x => x.Client.CrmId.Equals(parameter.ClientId));

                if (parameter.ReasonId.HasValue && parameter.ReasonId.Value > 0)
                    query = query.Where(x => x.ReasonCauseId == parameter.ReasonId.Value);

                if (parameter.RecruiterId.HasValue && parameter.RecruiterId.Value > 0)
                    query = query.Where(x => x.RecruiterId == parameter.RecruiterId.Value);

                if (parameter.UserId.HasValue && parameter.UserId.Value > 0)
                    query = query.Where(x => x.UserId == parameter.UserId.Value);

                if (parameter.Profiles != null && parameter.Profiles.Any())
                    query = query.Where(x => x.JobSearchProfiles.Any(s => parameter.Profiles.Contains(s.ProfileId)));

                if (parameter.Seniorities != null && parameter.Seniorities.Any())
                    query = query.Where(x => x.JobSearchSeniorities.Any(s => parameter.Seniorities.Contains(s.SeniorityId)));

                if (parameter.Skills != null && parameter.Skills.Any())
                    query = query.Where(x => x.JobSearchSkillsRequired.Any(s => parameter.Skills.Contains(s.SkillId)));
            }

            return query.ToList();
        }

        public JobSearch GetDetail(int id)
        {
            return context.JobSearchs
                .Include(x => x.JobSearchProfiles)
                .Include(x => x.JobSearchSeniorities)
                .Include(x => x.JobSearchSkillsRequired)
                .Include(x => x.JobSearchSkillsNotRequired)
                .Include(x => x.Client)
                .Include(x => x.TimeHiring)
                .SingleOrDefault(x => x.Id == id);
        }

        public JobSearch GetWithProfilesAndSkills(int jobSearchId)
        {
            return context.JobSearchs
                .Include(x => x.JobSearchProfiles)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.JobSearchSkillsRequired)
                    .ThenInclude(x => x.Skill)
                .SingleOrDefault(x => x.Id == jobSearchId);
        }

        public JobSearch GetWithProfilesAndSkillsAndSenorities(int id)
        {
            return context.JobSearchs
                .Include(x => x.JobSearchProfiles)
                .Include(x => x.JobSearchSeniorities)
                .Include(x => x.JobSearchSkillsRequired)
                .Include(x => x.JobSearchSkillsNotRequired)
                .SingleOrDefault(x => x.Id == id);
        }

        public void AddHistory(JobSearchHistory history)
        {
            context.JobSearchHistories.Add(history);
        }

        public IList<JobSearchHistory> GetHistory(int id)
        {
            return context.JobSearchHistories.Include(x => x.ReasonCause).Where(x => x.JobSearchId == id).ToList();
        }

        public IList<JobSearch> Get(List<int> skills, List<int> profiles)
        {
            var query = context.JobSearchs
                .Include(x => x.Client)
                .Include(x => x.JobSearchApplicants)
                    .ThenInclude(x => x.Reason)
                .Include(x => x.JobSearchProfiles)
                .Include(x => x.JobSearchSkillsRequired)
                .Where(x => x.Status == JobSearchStatus.Open || x.Status == JobSearchStatus.Reopen);

            if (skills.Any() && profiles.Any())
            {
                query = query.Where(x => x.JobSearchSkillsRequired.Any(s => skills.Contains(s.SkillId)) || x.JobSearchProfiles.Any(s => profiles.Contains(s.ProfileId)));
            }
            else
            {
                if (skills.Any())
                    query = query.Where(x => x.JobSearchSkillsRequired.Any(s => skills.Contains(s.SkillId)));

                if (profiles.Any())
                    query = query.Where(x => x.JobSearchProfiles.Any(s => skills.Contains(s.ProfileId)));
            }

            return query
                .Include(x => x.JobSearchApplicants)
                .Include(x => x.JobSearchProfiles)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.JobSearchSkillsRequired)
                    .ThenInclude(x => x.Skill)
                .Select(x => new JobSearch
                {
                    Id = x.Id,
                    JobSearchProfiles = x.JobSearchProfiles.Select(s => new JobSearchProfile
                    {
                        JobSearch = s.JobSearch,
                        Profile = s.Profile
                    })
                    .ToList(),
                    JobSearchSkillsRequired = x.JobSearchSkillsRequired.Select(s => new JobSearchSkillRequired
                    {
                        JobSearch = s.JobSearch,
                        Skill = s.Skill,
                    })
                    .ToList(),
                    Client = x.Client,
                    ReasonCause = x.ReasonCause,
                    JobSearchApplicants = x.JobSearchApplicants
                })
            .ToList();
        }
    }
}
