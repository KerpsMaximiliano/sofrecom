using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                .Include(x => x.ApplicantSkills)
                    .ThenInclude(x => x.Skill)
                .Include(x => x.ApplicantProfiles)
                    .ThenInclude(x => x.Profile);

            if (!string.IsNullOrWhiteSpace(parameter.FirstName))
            {
                parameter.FirstName = parameter.FirstName.ToLowerInvariant();
                var filtered = false;

                if (parameter.FirstName.Contains("-") && parameter.FirstName.Length == 3)
                {
                    query = query.Where(x => Regex.IsMatch(x.FirstName.ToLowerInvariant(), $"^[{parameter.FirstName}]"));
                    filtered = true;
                }

                if (parameter.FirstName.Contains(","))
                {
                    var split = parameter.FirstName.Split(',');

                    if (split.All(x => x.Length == 1))
                    {
                        parameter.FirstName = parameter.FirstName.Replace(",", "");
                        query = query.Where(x => Regex.IsMatch(x.FirstName.ToLowerInvariant(), $"^[{parameter.FirstName}]"));
                        filtered = true;
                    }
                }

                if (!filtered)
                {
                    query = query.Where(x => x.FirstName.ToLowerInvariant().StartsWith(parameter.FirstName));
                }
            }

            if (!string.IsNullOrWhiteSpace(parameter.LastName))
            {
                parameter.LastName = parameter.LastName.ToLowerInvariant();
                var filtered = false;

                if (parameter.LastName.Contains("-") && parameter.LastName.Length == 3)
                {
                    query = query.Where(x => Regex.IsMatch(x.LastName.ToLowerInvariant(), $"^[{parameter.LastName}]"));
                    filtered = true;
                }

                if (parameter.LastName.Contains(","))
                {
                    var split = parameter.LastName.Split(',');

                    if (split.All(x => x.Length == 1))
                    {
                        parameter.LastName = parameter.LastName.Replace(",", "");
                        query = query.Where(x => Regex.IsMatch(x.LastName.ToLowerInvariant(), $"^[{parameter.LastName}]"));
                        filtered = true;
                    }
                }

                if (!filtered)
                {
                    query = query.Where(x => x.LastName.ToLowerInvariant().StartsWith(parameter.LastName));
                }
            }

            if (parameter.Profiles != null && parameter.Profiles.Any())
                query = query.Where(x => x.ApplicantProfiles.Any(s => parameter.Profiles.Contains(s.ProfileId)));

            if (parameter.Skills != null && parameter.Skills.Any())
                query = query.Where(x => x.ApplicantSkills.Any(s => parameter.Skills.Contains(s.SkillId)));

            if (parameter.StatusIds != null && parameter.StatusIds.Any())
                query = query.Where(x => parameter.StatusIds.Contains((int) x.Status));

            return query.ToList();
        }

        public Applicant GetDetail(int id)
        {
            return context.Applicants
                .Include(x => x.ApplicantProfiles)
                .Include(x => x.ApplicantSkills)
                .SingleOrDefault(x => x.Id == id);
        }

        public IList<JobSearchApplicant> GetHistory(int applicantId)
        {
            return context.JobSearchApplicants
                .Include(x => x.Applicant)
                .Include(x => x.Reason)
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

        public IList<ApplicantFile> GetFiles(int id)
        {
            return context.ApplicantFiles
                .Include(x => x.File)
                .Where(x => x.ApplicantId == id)
                .ToList();
        }

        public Applicant GetWithProfilesAndSkills(int applicantId)
        {
            return context.Applicants
                .Include(x => x.ApplicantSkills)
                .SingleOrDefault(x => x.Id == applicantId);
        }
    }
}
