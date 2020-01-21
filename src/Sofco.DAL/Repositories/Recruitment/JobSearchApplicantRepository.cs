using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Recruitment;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
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
            var query = context.Applicants
                .Include(x => x.JobSearchApplicants)
                    .ThenInclude(x => x.Reason)
                .Include(x => x.ApplicantProfiles)
                .Include(x => x.ApplicantSkills)
                .Where(x => x.Status == ApplicantStatus.Valid || x.Status == ApplicantStatus.InProgress || x.Status == ApplicantStatus.Contacted);

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
                    JobSearchApplicants = x.JobSearchApplicants.Select(s => new JobSearchApplicant
                    {
                        JobSearchId = s.JobSearchId,
                        Reason = new ReasonCause
                        {
                            Id = s.ReasonId,
                            Type = s.Reason.Type
                        },
                        CreatedDate = s.CreatedDate
                    })
                    .OrderByDescending(s => s.CreatedDate).ToList(),
                    FirstName = x.FirstName,
                    DocumentNumber = x.DocumentNumber,
                    LastName = x.LastName,
                })
            .ToList();
        }

        public JobSearchApplicant GetById(int applicantId, int jobSearchId, DateTime date, int reasonId)
        {
            return context.JobSearchApplicants.SingleOrDefault(x => x.JobSearchId == jobSearchId && x.ApplicantId == applicantId && x.CreatedDate.Date == date.Date && x.ReasonId == reasonId);
        }

        public void InsertFile(ApplicantFile jobsearchApplicantFile)
        {
            context.ApplicantFiles.Add(jobsearchApplicantFile);
        }

        public bool Exist(int applicantId, int jobSearchId, DateTime date, int reasonId)
        {
            return context.JobSearchApplicants.Any(x => x.JobSearchId == jobSearchId && x.ApplicantId == applicantId && x.CreatedDate.Date == date.Date && x.ReasonId == reasonId);
        }

        public IList<JobSearchApplicant> GetByApplicant(int applicantId)
        {
            return context.JobSearchApplicants.Include(x => x.Reason).Where(x => x.ApplicantId == applicantId).ToList();
        }

        public IList<JobSearchApplicant> GetWithInterviewAfterToday()
        {
            var today = DateTime.UtcNow.Date;

            return context.JobSearchApplicants
                .Include(x => x.Reason)
                .Include(x => x.Applicant)
                .Include(x => x.RrhhInterviewer)
                .Where(x => x.Reason.Type == ReasonCauseType.ApplicantInProgress &&
                            (x.RrhhInterviewDate.HasValue && x.RrhhInterviewDate.Value.AddDays(-1).Date == today.Date) ||
                            x.TechnicalInterviewDate.HasValue && x.TechnicalInterviewDate.Value.AddDays(-1).Date == today.Date ||
                            x.ClientInterviewDate.HasValue && x.ClientInterviewDate.Value.AddDays(-1).Date == today.Date)
                .ToList();
        }

        public IList<JobSearchApplicant> GetWithInterviewABeforeToday()
        {
            var today = DateTime.UtcNow.Date;

            return context.JobSearchApplicants
                .Include(x => x.Reason)
                .Include(x => x.Applicant)
                .Include(x => x.RrhhInterviewer)
                .Where(x => x.Reason.Type == ReasonCauseType.ApplicantInProgress &&
                            (x.RrhhInterviewDate.HasValue && x.RrhhInterviewDate.Value.Date < today.Date && x.ModifiedAt.Date < x.RrhhInterviewDate.Value.Date) ||
                            (x.TechnicalInterviewDate.HasValue && x.TechnicalInterviewDate.Value.Date < today.Date && x.ModifiedAt.Date < x.TechnicalInterviewDate.Value.Date) ||
                            (x.ClientInterviewDate.HasValue && x.ClientInterviewDate.Value.Date < today.Date && x.ModifiedAt.Date < x.ClientInterviewDate.Value.Date))
                .ToList();
        }
    }
}
