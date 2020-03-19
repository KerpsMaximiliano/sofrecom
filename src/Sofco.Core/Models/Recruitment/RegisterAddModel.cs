using System;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class RegisterAddModel
    {
        public string Nationality { get; set; }

        public string CivilStatus { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? StartDate { get; set; }

        public string Address { get; set; }

        public string Cuil { get; set; }

        public string Prepaid { get; set; }

        public int? ProfileId { get; set; }

        public string Office { get; set; }

        public string Aggreements { get; set; }

        public decimal? Salary { get; set; }

        public int? ManagerId { get; set; }

        public int? AnalyticId { get; set; }

        public int? ProjectId { get; set; }

        public int? SkillId { get; set; }

        public int? SeniorityId { get; set; }

        public void UpdateDomain(Applicant applicant)
        {
            applicant.Nationality = Nationality;
            applicant.Aggreements = Aggreements;
            applicant.CivilStatus = CivilStatus;
            applicant.Address = Address;
            applicant.Cuil = Cuil;
            applicant.Prepaid = Prepaid;
            applicant.ProfileId = ProfileId;
            applicant.Office = Office;
            applicant.Salary = Salary;
            applicant.ManagerId = ManagerId;
            applicant.AnalyticId = AnalyticId;
            applicant.ProjectId = ProjectId;
            applicant.BirthDate = BirthDate;
            applicant.StartDate = StartDate;
            applicant.SkillId = SkillId;
            applicant.SeniorityId = SeniorityId;
        }
    }

    public class RegisterModel
    {
        public ApplicantAddModel GeneralData { get; set; }

        public RegisterAddModel RegisterData { get; set; }
    }
}
