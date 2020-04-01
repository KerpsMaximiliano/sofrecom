using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchChangeStatusModel
    {
        public JobSearchStatus? Status { get; set; }

        public string Reason { get; set; }

        public int? ReasonCauseId { get; set; }
    }
}
