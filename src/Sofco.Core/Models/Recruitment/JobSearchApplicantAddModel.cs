using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchApplicantAddModel
    {
        public IList<int> Applicants { get; set; }

        public int? ReasonId { get; set; }

        public string Comments { get; set; }

        public int JobSearchId { get; set; }
    }
}
