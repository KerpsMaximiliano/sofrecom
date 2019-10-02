using System;

namespace Sofco.Domain.Models.Recruitment
{
    public class JobSearchApplicant
    {
        public int JobSearchId { get; set; }

        public JobSearch JobSearch { get; set; }

        public int ApplicantId { get; set; }

        public Applicant Applicant { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ReasonId { get; set; }

        public ReasonCause Reason { get; set; }

        public string Comments { get; set; }

        public string CreatedBy { get; set; }
    }
}
