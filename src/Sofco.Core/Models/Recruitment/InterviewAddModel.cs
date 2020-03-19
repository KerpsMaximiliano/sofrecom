using System;

namespace Sofco.Core.Models.Recruitment
{
    public class InterviewAddModel
    {
        public int ReasonId { get; set; }
        public decimal? Salary { get; set; }

        public bool RemoteWork { get; set; }

        public bool HasPhoneInterview { get; set; }

        public bool HasRrhhInterview { get; set; }

        public DateTime? RrhhInterviewDate { get; set; }

        public string RrhhInterviewPlace { get; set; }

        public int? RrhhInterviewerId { get; set; }

        public bool HasTechnicalInterview { get; set; }

        public DateTime? TechnicalInterviewDate { get; set; }

        public string TechnicalInterviewPlace { get; set; }

        public string TechnicalExternalInterviewer { get; set; }

        public string TechnicalInterviewComments { get; set; }

        public bool HasClientInterview { get; set; }

        public DateTime? ClientInterviewDate { get; set; }

        public string ClientInterviewPlace { get; set; }

        public string ClientInterviewComments { get; set; }

        public string ClientExternalInterviewer { get; set; }

        public string RrhhInterviewComments { get; set; }
    }
}
