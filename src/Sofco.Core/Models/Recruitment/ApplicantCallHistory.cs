using System;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantCallHistory
    {
        public ApplicantCallHistory(JobSearchApplicant domain)
        {
            Date = domain.CreatedDate;
            CreatedBy = domain.CreatedBy;
            Comments = domain.Comments;
            JobSearchId = domain.JobSearchId;

            if (domain.Reason != null)
            {
                Reason = domain.Reason.Text;
                ReasonType = domain.Reason.Type;
            }

            if (domain.JobSearch.Client != null) Client = domain.JobSearch.Client.Name;

            if (domain.JobSearch.JobSearchProfiles != null && domain.JobSearch.JobSearchProfiles.Any())
                Profiles = string.Join(";", domain.JobSearch.JobSearchProfiles.Select(x => x.Profile.Text));

            if (domain.JobSearch.JobSearchSkillsRequired != null && domain.JobSearch.JobSearchSkillsRequired.Any())
                Skills = string.Join(";", domain.JobSearch.JobSearchSkillsRequired.Select(x => x.Skill.Text));

            HasRrhhInterview = domain.HasRrhhInterview;
            RrhhInterviewDate = domain.RrhhInterviewDate;
            RrhhInterviewPlace = domain.RrhhInterviewPlace;
            RrhhInterviewerId = domain.RrhhInterviewerId;

            HasTechnicalInterview = domain.HasTechnicalInterview;
            TechnicalInterviewDate = domain.TechnicalInterviewDate;
            TechnicalInterviewPlace = domain.TechnicalInterviewPlace;
            TechnicalInterviewerId = domain.TechnicalInterviewerId;

            HasClientInterview = domain.HasClientInterview;
            ClientInterviewDate = domain.ClientInterviewDate;
            ClientInterviewPlace = domain.ClientInterviewPlace;
            ClientInterviewerId = domain.ClientInterviewerId;
        }

        public ReasonCauseType ReasonType { get; set; }

        public int? ClientInterviewerId { get; set; }

        public string ClientInterviewPlace { get; set; }

        public DateTime? ClientInterviewDate { get; set; }

        public bool HasClientInterview { get; set; }

        public int? TechnicalInterviewerId { get; set; }

        public string TechnicalInterviewPlace { get; set; }

        public DateTime? TechnicalInterviewDate { get; set; }

        public bool HasTechnicalInterview { get; set; }

        public int? RrhhInterviewerId { get; set; }

        public DateTime? RrhhInterviewDate { get; set; }

        public string RrhhInterviewPlace { get; set; }

        public bool HasRrhhInterview { get; set; }

        public DateTime Date { get; set; }

        public string Reason { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public string Client { get; set; }

        public int JobSearchId { get; set; }
    }
}
