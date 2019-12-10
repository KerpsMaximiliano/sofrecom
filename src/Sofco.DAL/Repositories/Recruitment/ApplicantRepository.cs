using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Recruitment;
using Sofco.Core.Models.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Repositories.Recruitment
{
    public class ApplicantRepository : BaseRepository<Applicant>, IApplicantRepository
    {
        public ApplicantRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Applicant> Search(ApplicantSearchParameters parameter)
        {
            IQueryable<Applicant> query = context.Applicants
                .Include(x => x.Client)
                .Include(x => x.ApplicantSkills)
                    .ThenInclude(x => x.Skill)
                .Include(x => x.ApplicantProfiles)
                    .ThenInclude(x => x.Profile);

            if (!string.IsNullOrWhiteSpace(parameter.FirstName))
                query = query.Where(x => x.FirstName.ToLowerInvariant().Contains(parameter.FirstName.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameter.LastName))
                query = query.Where(x => x.LastName.ToLowerInvariant().Contains(parameter.LastName.ToLowerInvariant()));

            if (!string.IsNullOrWhiteSpace(parameter.ClientCrmId))
                query = query.Where(x => x.Client.CrmId.Equals(parameter.ClientCrmId));

            if (parameter.Profiles != null && parameter.Profiles.Any())
                query = query.Where(x => x.ApplicantProfiles.Any(s => parameter.Profiles.Contains(s.ProfileId)));

            if (parameter.Skills != null && parameter.Skills.Any())
                query = query.Where(x => x.ApplicantSkills.Any(s => parameter.Skills.Contains(s.SkillId)));

            return query.ToList();
        }

        public Applicant GetDetail(int id)
        {
            return context.Applicants
                .Include(x => x.ApplicantProfiles)
                .Include(x => x.ApplicantSkills)
                .Include(x => x.Client)
                .SingleOrDefault(x => x.Id == id);
        }

        public IList<JobSearchApplicant> GetHistory(int applicantId)
        {
            return context.JobSearchApplicants
                .Include(x => x.Applicant)
                .Include(x => x.Reason)
                .Include(x => x.Files)
                    .ThenInclude(x => x.File)
                .Include(x => x.JobSearch)
                    .ThenInclude(x => x.Client)
                .Include(x => x.JobSearch)
                    .ThenInclude(x => x.JobSearchProfiles)
                        .ThenInclude(x => x.Profile)
                .Include(x => x.JobSearch)
                    .ThenInclude(x => x.JobSearchSkillsRequired)
                        .ThenInclude(x => x.Skill)
                .Where(x => x.ApplicantId == applicantId)
                .ToList();
        }

        public void AddHistory(ApplicantHistory history)
        {
            context.ApplicantHistories.Add(history);
        }
    }
}
