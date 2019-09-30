using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Repositories.Recruitment
{
    class JobSearchApplicantRepository : BaseRepository<JobSearchApplicant>, IJobSearchApplicantRepository
    {
        public JobSearchApplicantRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Applicant> Get(IList<int> skills, IList<int> profiles)
        {
            IQueryable<Applicant> query = context.Applicants
                .Include(x => x.JobSearchApplicants)
                .Include(x => x.ApplicantProfiles)
                .Include(x => x.ApplicantSkills);

            if (skills.Any() && profiles.Any())
            {
                query = query.Where(x => x.ApplicantSkills.Any(s => skills.Contains(s.SkillId)) || x.ApplicantProfiles.Any(s => profiles.Contains(s.ProfileId)));
            }
            else
            {
                if (skills.Any())
                    query = query.Where(x => x.ApplicantSkills.Any(s => skills.Contains(s.SkillId)));

                if (profiles.Any())
                    query = query.Where(x => x.ApplicantProfiles.Any(s => skills.Contains(s.ProfileId)));
            }

            return query.Select(x => new Applicant
            {
                ApplicantProfiles = x.ApplicantProfiles,
                ApplicantSkills = x.ApplicantSkills,
                FirstName = x.FirstName,
                LastName = x.LastName,
                JobSearchApplicants = x.JobSearchApplicants.OrderByDescending(s => s.CreatedDate).Take(1).ToList(),
            }).ToList();
        }
    }
}
