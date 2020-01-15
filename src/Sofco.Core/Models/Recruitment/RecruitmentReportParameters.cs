using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Recruitment
{
    public class RecruitmentReportParameters
    {
        public string ClientId { get; set; }

        public IList<int> Skills { get; set; }

        public IList<JobSearchStatus> Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
