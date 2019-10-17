using System;
using Sofco.Domain.Enums;

namespace Sofco.Domain.Models.Recruitment
{
    public class JobSearchHistory : BaseEntity
    {
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }

        public JobSearchStatus StatusFromId { get; set; }

        public JobSearchStatus StatusToId { get; set; }

        public string Comment { get; set; }

        public int ReasonCauseId { get; set; }

        public ReasonCause ReasonCause { get; set; }

        public int JobSearchId { get; set; }

        public JobSearch JobSearch { get; set; }
    }
}
