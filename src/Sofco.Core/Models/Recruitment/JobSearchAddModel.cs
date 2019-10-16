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

        public int? ClientId { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> SkillsRequired { get; set; }

        public IList<int> SkillsNotRequired { get; set; }

        public IList<int> Seniorities { get; set; }

        public int? Quantity { get; set; }

        public int? TimeHiringId { get; set; }

        public decimal? MaximunSalary { get; set; }

        public string Comments { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string ClientContact { get; set; }

        public string Language { get; set; }

        public string Study { get; set; }

        public string JobTime { get; set; }

        public string Location { get; set; }

        public string Benefits { get; set; }

        public string Observations { get; set; }

        public string TasksToDo { get; set; }

        public string Area { get; set; }

        public bool HasExtraHours { get; set; }

        public bool ExtraHoursPaid { get; set; }

        public bool HasGuards { get; set; }

        public bool GuardsPaid { get; set; }

        public int JobType { get; set; }

        public int ResourceAssignment { get; set; }

        public int? YearsExperience { get; set; }

        public bool StudyRequired { get; set; }

        public bool LanguageRequired { get; set; }

        public bool IsMarketStudy { get; set; }

        public string MarketStudy { get; set; }

        public JobSearch CreateDomain()
        {
            var domain = new JobSearch();

            SetData(domain);

            if (Profiles != null && Profiles.Any())
            {
                domain.JobSearchProfiles = Profiles.Select(x => new JobSearchProfile { ProfileId = x }).ToList();
            }

            if (Seniorities != null && Seniorities.Any())
            {
                domain.JobSearchSeniorities = Seniorities.Select(x => new JobSearchSeniority { SeniorityId = x }).ToList();
            }

            if (SkillsRequired != null && SkillsRequired.Any())
            {
                domain.JobSearchSkillsRequired = SkillsRequired.Select(x => new JobSearchSkillRequired { SkillId = x }).ToList();
            }

            if (SkillsNotRequired != null && SkillsNotRequired.Any())
            {
                domain.JobSearchSkillsNotRequired = SkillsNotRequired.Select(x => new JobSearchSkillNotRequired { SkillId = x }).ToList();
            }

            domain.Status = JobSearchStatus.Open;

            return domain;
        }

        public void UpdateDomain(JobSearch domain)
        {
            SetData(domain);

            if (Profiles == null || !Profiles.Any())
            {
                domain.JobSearchProfiles = new List<JobSearchProfile>();
            }
            else
            {
                domain.JobSearchProfiles = Profiles.Select(x => new JobSearchProfile { ProfileId = x, JobSearchId = domain.Id }).ToList();
            }

            if (Seniorities == null || !Seniorities.Any())
            {
                domain.JobSearchSeniorities = new List<JobSearchSeniority>();
            }
            else
            {
                domain.JobSearchSeniorities = Seniorities.Select(x => new JobSearchSeniority { SeniorityId = x, JobSearchId = domain.Id }).ToList();
            }

            if (SkillsRequired == null || !SkillsRequired.Any())
            {
                domain.JobSearchSkillsRequired = new List<JobSearchSkillRequired>();
            }
            else
            {
                domain.JobSearchSkillsRequired = SkillsRequired.Select(x => new JobSearchSkillRequired { SkillId = x, JobSearchId = domain.Id }).ToList();
            }

            if (SkillsNotRequired == null || !SkillsNotRequired.Any())
            {
                domain.JobSearchSkillsNotRequired = new List<JobSearchSkillNotRequired>();
            }
            else
            {
                domain.JobSearchSkillsNotRequired = SkillsNotRequired.Select(x => new JobSearchSkillNotRequired { SkillId = x, JobSearchId = domain.Id }).ToList();
            }
        }

        private void SetData(JobSearch domain)
        {
            domain.ClientId = ClientId;
            domain.ReasonCauseId = ReasonCauseId.GetValueOrDefault();
            domain.UserId = UserId.GetValueOrDefault();
            domain.Comments = Comments;
            domain.MaximunSalary = MaximunSalary.GetValueOrDefault();
            domain.Quantity = Quantity.GetValueOrDefault();
            domain.TimeHiringId = TimeHiringId.GetValueOrDefault();
            domain.RecruiterId = RecruiterId;
            domain.YearsExperience = YearsExperience;
            domain.Email = Email;
            domain.Telephone = Telephone;
            domain.ClientContact = ClientContact;
            domain.JobType = JobType;
            domain.ResourceAssignmentId = ResourceAssignment;
            domain.Language = Language;
            domain.Study = Study;
            domain.JobTime = JobTime;
            domain.Location = Location;
            domain.Benefits = Benefits;
            domain.Observations = Observations;
            domain.TasksToDo = TasksToDo;
            domain.HasExtraHours = HasExtraHours;
            domain.ExtraHoursPaid = ExtraHoursPaid;
            domain.HasGuards = HasGuards;
            domain.GuardsPaid = GuardsPaid;
            domain.Area = Area;
            domain.LanguageRequired = LanguageRequired;
            domain.StudyRequired = StudyRequired;
            domain.MarketStudy = MarketStudy;
            domain.IsMarketStudy = IsMarketStudy;
        }
    }
}
