using System;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchApplicantModel
    {
        public JobSearchApplicantModel(Applicant applicant)
        {
            Applicant = $"{applicant.FirstName} {applicant.LastName}";
        }

        public string Applicant { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public DateTime Date { get; set; }
    }
}
