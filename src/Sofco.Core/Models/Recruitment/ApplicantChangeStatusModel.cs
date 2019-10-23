using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantChangeStatusModel
    {
        public ApplicantStatus? Status { get; set; }

        public string Comments { get; set; }

        public int? ReasonCauseId { get; set; }
    }
}
