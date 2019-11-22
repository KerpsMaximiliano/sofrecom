using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;
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
            MaximunSalary = jobsearch.MaximunSalary;
            Comments = jobsearch.Comments;
            Status = jobsearch.Status;
            Telephone = jobsearch.Telephone;
            ClientContact = jobsearch.ClientContact;
            Language = jobsearch.Language;
            Study = jobsearch.Study;
            JobTime = jobsearch.JobTime;
            Location = jobsearch.Location;
            Benefits = jobsearch.Benefits;
            Observations = jobsearch.Observations;
            TasksToDo = jobsearch.TasksToDo;
            Area = jobsearch.Area;
            HasExtraHours = jobsearch.HasExtraHours;
            ExtraHoursPaid = jobsearch.ExtraHoursPaid;
            HasGuards = jobsearch.HasGuards;
            GuardsPaid = jobsearch.GuardsPaid;
            Email = jobsearch.Email;
            JobType = jobsearch.JobType;
            ResourceAssignment = jobsearch.ResourceAssignmentId;
            YearsExperience = jobsearch.YearsExperience;
            LanguageRequired = jobsearch.LanguageRequired;
            StudyRequired = jobsearch.StudyRequired;
            IsMarketStudy = jobsearch.IsMarketStudy;
            MarketStudy = jobsearch.MarketStudy;
            IsStaffDesc = jobsearch.IsStaffDesc;
            IsStaff = jobsearch.IsStaff;

            if (jobsearch.Client != null) ClientCrmId = jobsearch.Client.CrmId;

            if (jobsearch.JobSearchProfiles != null && jobsearch.JobSearchProfiles.Any())
                Profiles = jobsearch.JobSearchProfiles.Select(x => x.ProfileId).ToList();

            if (jobsearch.JobSearchSeniorities != null && jobsearch.JobSearchSeniorities.Any())
                Seniorities = jobsearch.JobSearchSeniorities.Select(x => x.SeniorityId).ToList();

            if (jobsearch.JobSearchSkillsRequired != null && jobsearch.JobSearchSkillsRequired.Any())
                SkillsRequired = jobsearch.JobSearchSkillsRequired.Select(x => x.SkillId).ToList();

            if (jobsearch.JobSearchSkillsNotRequired != null && jobsearch.JobSearchSkillsNotRequired.Any())
                SkillsNotRequired = jobsearch.JobSearchSkillsNotRequired.Select(x => x.SkillId).ToList();

            if (jobsearch.TimeHiring != null) TimeHiringId = jobsearch.TimeHiringId;
        }

        public string IsStaffDesc { get; set; }

        public bool IsStaff { get; set; }

        public string MarketStudy { get; set; }

        public bool IsMarketStudy { get; set; }

        public bool StudyRequired { get; set; }

        public bool LanguageRequired { get; set; }

        public JobSearchStatus Status { get; set; }

        public int TimeHiringId { get; set; }

        public int UserId { get; set; }

        public int? RecruiterId { get; set; }

        public int ReasonCauseId { get; set; }

        public string ClientCrmId { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> SkillsRequired { get; set; }

        public IList<int> SkillsNotRequired { get; set; }

        public IList<int> Seniorities { get; set; }

        public int Quantity { get; set; }

        public decimal MaximunSalary { get; set; }

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
    }
}
