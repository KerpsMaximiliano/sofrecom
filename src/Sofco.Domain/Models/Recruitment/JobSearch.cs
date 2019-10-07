using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;

namespace Sofco.Domain.Models.Recruitment
{
    public class JobSearch : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RecruiterId { get; set; }
        public User Recruiter { get; set; }

        public int ReasonCauseId { get; set; }
        public ReasonCause ReasonCause { get; set; }

        public int ClientId { get; set; }
        public Customer Client { get; set; }

        public IList<JobSearchProfile> JobSearchProfiles { get; set; }

        public IList<JobSearchSkillRequired> JobSearchSkillsRequired { get; set; }

        public IList<JobSearchSkillNotRequired> JobSearchSkillsNotRequired { get; set; }

        public IList<JobSearchSeniority> JobSearchSeniorities { get; set; }

        public int Quantity { get; set; }

        public decimal MaximunSalary { get; set; }

        public string Comments { get; set; }

        public JobSearchStatus Status { get; set; }

        public DateTime? ReopenDate { get; set; }

        public DateTime? SuspendedDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ReasonComments { get; set; }

        public int TimeHiringId { get; set; }
        public TimeHiring TimeHiring { get; set; }

        public int YearsExperience { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string ClientContact { get; set; }

        public int JobType { get; set; }

        public int ResourceAssignment { get; set; }

        public string Language { get; set; }

        public bool LanguageRequired { get; set; }

        public string Study { get; set; }

        public bool StudyRequired { get; set; }

        public string JobTime { get; set; }

        public string Location { get; set; }

        public string Benefits { get; set; }

        public string Observations { get; set; }

        public string TasksToDo { get; set; }

        public bool HasExtraHours { get; set; }

        public bool ExtraHoursPaid { get; set; }

        public bool HasGuards { get; set; }

        public bool GuardsPaid { get; set; }

        public string Area { get; set; }

        public IList<JobSearchApplicant> JobSearchApplicants { get; set; }
    }
}
