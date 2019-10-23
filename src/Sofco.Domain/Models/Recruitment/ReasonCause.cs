using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class ReasonCause : Option
    {
        public ReasonCauseType Type { get; set; }
        public IList<JobSearch> JobSearchs { get; set; }
        public IList<Applicant> Applicants { get; set; }
        public IList<JobSearchApplicant> JobSearchApplicants { get; set; }
        public IList<JobSearchHistory> JobSearchHistories { get; set; }
        public IList<ApplicantHistory> ApplicantHistories { get; set; }
    }
}
