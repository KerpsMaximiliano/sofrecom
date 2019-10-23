﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Relationships;

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

            return query
                .Include(x => x.ApplicantProfiles)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.ApplicantSkills)
                    .ThenInclude(x => x.Skill)
                .Select(x => new Applicant
                {
                    Id = x.Id,
                    ApplicantProfiles = x.ApplicantProfiles.Select(s => new ApplicantProfile
                    {
                        Applicant = s.Applicant,
                        Profile = s.Profile
                    })
                    .ToList(),
                    ApplicantSkills = x.ApplicantSkills.Select(s => new ApplicantSkills
                    {
                        Applicant = s.Applicant,
                        Skill = s.Skill,
                    })
                    .ToList(),
                    FirstName = x.FirstName,
                    DocumentNumber = x.DocumentNumber,
                    LastName = x.LastName,
                    JobSearchApplicants = x.JobSearchApplicants.OrderByDescending(s => s.CreatedDate).Take(1).ToList(),
                })
            .ToList();
        }
    }
}
