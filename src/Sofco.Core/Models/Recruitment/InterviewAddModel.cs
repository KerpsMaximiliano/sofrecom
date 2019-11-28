using System;

namespace Sofco.Core.Models.Recruitment
{
    public class InterviewAddModel
    {
        public int ReasonId { get; set; }

        public bool HasRrhhInterview { get; set; }

        public DateTime? RrhhInterviewDate { get; set; }

        public string RrhhInterviewPlace { get; set; }

        public int? RrhhInterviewerId { get; set; }

        public bool HasTechnicalInterview { get; set; }

        public DateTime? TechnicalInterviewDate { get; set; }

        public string TechnicalInterviewPlace { get; set; }

        public int? TechnicalInterviewerId { get; set; }

        public bool HasClientInterview { get; set; }

        public DateTime? ClientInterviewDate { get; set; }

        public string ClientInterviewPlace { get; set; }

        public int? ClientInterviewerId { get; set; }
    }
}
