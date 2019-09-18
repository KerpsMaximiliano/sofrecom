using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Recruitment;
using Sofco.Core.Models.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;

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
                .Include(x => x.JobSearchSkills)
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
                    query = query.Where(x => x.JobSearchSkills.Any(s => parameter.Skills.Contains(s.SkillId)));
            }

            return query.ToList();
        }

        public JobSearch GetDetail(int id)
        {
            return context.JobSearchs
                .Include(x => x.JobSearchProfiles)
                .Include(x => x.JobSearchSeniorities)
                .Include(x => x.JobSearchSkills)
                .Include(x => x.Client)
                .SingleOrDefault(x => x.Id == id);
        }
    }
}
