using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantDetailModel
    {
        public ApplicantDetailModel(Applicant applicant)
        {
            LastName = applicant.LastName;
            FirstName = applicant.FirstName;
            Email = applicant.Email;
            Comments = applicant.Comments;
            ReasonCauseId = applicant.ReasonCauseId;
            CountryCode1 = applicant.CountryCode1;
            AreaCode1 = applicant.AreaCode1;
            Telephone1 = applicant.Telephone1;
            CountryCode2 = applicant.CountryCode2;
            AreaCode2 = applicant.AreaCode2;
            Telephone2 = applicant.Telephone2;
            Nationality = applicant.Nationality;
            CivilStatus = applicant.CivilStatus;
            BirthDate = applicant.BirthDate;
            StartDate = applicant.StartDate;
            Address = applicant.Address;
            Cuil = applicant.Cuil;
            Prepaid = applicant.Prepaid;
            ProfileId = applicant.ProfileId;
            Office = applicant.Office;
            Agreements = applicant.Aggreements;
            Salary = applicant.Salary;
            ManagerId = applicant.ManagerId;
            AnalyticId = applicant.AnalyticId;
            ProjectId = applicant.ProjectId;
            Status = applicant.Status;

            if (applicant.RecommendedByUserId.HasValue) RecommendedByUserId = applicant.RecommendedByUserId.Value.ToString();

            if (applicant.Client != null) ClientId = applicant.Client.CrmId;

            if (applicant.ApplicantProfiles != null && applicant.ApplicantProfiles.Any())
                Profiles = applicant.ApplicantProfiles.Select(x => x.ProfileId).ToList();

            if (applicant.ApplicantSkills != null && applicant.ApplicantSkills.Any())
                Skills = applicant.ApplicantSkills.Select(x => x.SkillId).ToList();
        }

        public string Prepaid { get; set; }

        public int? ProfileId { get; set; }

        public int? ProjectId { get; set; }

        public string Address { get; set; }

        public ApplicantStatus Status { get; set; }

        public int? AnalyticId { get; set; }

        public int? ManagerId { get; set; }

        public decimal? Salary { get; set; }

        public string Agreements { get; set; }

        public string Office { get; set; }

        public string Cuil { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? BirthDate { get; set; }

        public string CivilStatus { get; set; }

        public string Nationality { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string Comments { get; set; }

        public int? ReasonCauseId { get; set; }

        public string RecommendedByUserId { get; set; }

        public string CountryCode1 { get; set; }

        public string AreaCode1 { get; set; }

        public string Telephone1 { get; set; }

        public string CountryCode2 { get; set; }

        public string AreaCode2 { get; set; }

        public string Telephone2 { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Profiles { get; set; }

        public string ClientId { get; set; }
    }
}
