using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
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

        public int? RecommendedByUserId { get; set; }
        public User RecommendedByUser { get; set; }

        public string CountryCode1 { get; set; }

        public string AreaCode1 { get; set; }

        public string Telephone1 { get; set; }

        public string CountryCode2 { get; set; }

        public string AreaCode2 { get; set; }

        public string DocumentNumber { get; set; }

        public string Telephone2 { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public ApplicantStatus Status { get; set; }

        public IList<JobSearchApplicant> JobSearchApplicants { get; set; }

        public string Nationality { get; set; }

        public string CivilStatus { get; set; }

        public string Address { get; set; }

        public string Cuil { get; set; }

        public string Prepaid { get; set; }

        public int? ProfileId { get; set; }
        public Profile Profile { get; set; }

        public string Office { get; set; }

        public decimal? Salary { get; set; }

        public int? ManagerId { get; set; }
        public User Manager { get; set; }

        public int? AnalyticId { get; set; }
        public Analytic Analytic { get; set; }

        public int? ProjectId { get; set; }
        public Project Project { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? StartDate { get; set; }

        public string Aggreements { get; set; }

        public IList<ApplicantFile> Files { get; set; }
    }
}
