using System;
using System.Collections.Generic;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;

namespace Sofco.Domain.Models.Recruitment
{
    public class Applicant : BaseEntity
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Comments { get; set; }

        public string Email { get; set; }

        public IList<ApplicantProfile> ApplicantProfiles { get; set; }

        public IList<ApplicantSkills> ApplicantSkills { get; set; }

        public int? ClientId { get; set; }
        public Customer Client { get; set; }

        public int? ReasonCauseId { get; set; }
        public ReasonCause ReasonCause { get; set; }

        public int? RecommendedByUserId { get; set; }
        public User RecommendedByUser { get; set; }

        public string CountryCode1 { get; set; }

        public string AreaCode1 { get; set; }

        public string Telephone1 { get; set; }

        public string CountryCode2 { get; set; }

        public string AreaCode2 { get; set; }

        public string Telephone2 { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public IList<JobSearchApplicant> JobSearchApplicants { get; set; }
    }
}
