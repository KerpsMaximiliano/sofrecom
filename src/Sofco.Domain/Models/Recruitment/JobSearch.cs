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

        public IList<JobSearchSkill> JobSearchSkills { get; set; }

        public IList<JobSearchSeniority> JobSearchSeniorities { get; set; }

        public int Quantity { get; set; }

        public string TimeHiring { get; set; }

        public decimal MaximunSalary { get; set; }

        public string Comments { get; set; }

        public JobSearchStatus Status { get; set; }

        public DateTime? ReopenDate { get; set; }

        public DateTime? SuspendedDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ReasonComments { get; set; }
    }
}
