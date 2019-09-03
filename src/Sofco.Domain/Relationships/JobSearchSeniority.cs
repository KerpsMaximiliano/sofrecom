using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Relationships
{
    public class JobSearchSeniority
    {
        public int JobSearchId { get; set; }
        public JobSearch JobSearch { get; set; }

        public int SeniorityId { get; set; }
        public Seniority Seniority { get; set; }
    }
}
