using System;
using Sofco.Domain.Enums;

namespace Sofco.Domain.Models.Recruitment
{
    public class ApplicantHistory : BaseEntity
    {
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public ApplicantStatus StatusFromId { get; set; }

        public ApplicantStatus StatusToId { get; set; }

        public string Comment { get; set; }

        public int ReasonCauseId { get; set; }

        public ReasonCause ReasonCause { get; set; }

        public int ApplicantId { get; set; }

        public Applicant Applicant { get; set; }
    }
}
