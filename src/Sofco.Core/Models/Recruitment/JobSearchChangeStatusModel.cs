using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchChangeStatusModel
    {
        public DateTime? Date { get; set; }

        public JobSearchStatus? Status { get; set; }

        public string Reason { get; set; }
    }
}
