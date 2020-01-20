using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class RecruitmentReportResponse
    {
        public RecruitmentReportResponse(JobSearch jobSearch)
        {
            Code = jobSearch.Id;
            Status = jobSearch.Status;
            Quantity = jobSearch.Quantity;
            MaximumSalary = jobSearch.MaximunSalary;
            CreationDate = jobSearch.CreatedDate;

            if (jobSearch.Client != null)
            {
                Client = jobSearch.Client.Name;
            }

            if (jobSearch.ReasonCause != null)
            {
                ReasonId = jobSearch.ReasonCauseId;
                ReasonText = jobSearch.ReasonCause.Text;
                ReasonCauseType = jobSearch.ReasonCause.Type;
            }

            if (jobSearch.User != null)
            {
                UserId = jobSearch.UserId;
                UserText = jobSearch.User.Name;
            }

            if (jobSearch.JobSearchProfiles != null && jobSearch.JobSearchProfiles.Any())
                Profiles = string.Join(";", jobSearch.JobSearchProfiles.Select(x => x.Profile.Text));

            if (jobSearch.JobSearchSkillsRequired != null && jobSearch.JobSearchSkillsRequired.Any())
                Skills = string.Join(";", jobSearch.JobSearchSkillsRequired.Select(x => x.Skill.Text));

            if (jobSearch.JobSearchSeniorities != null && jobSearch.JobSearchSeniorities.Any())
                Seniorities = string.Join(";", jobSearch.JobSearchSeniorities.Select(x => x.Seniority.Text));

            Contacts = new List<RecruitmentContactReport>();

            if (jobSearch.JobSearchApplicants != null && jobSearch.JobSearchApplicants.Any())
            {
                Contacts = jobSearch.JobSearchApplicants.Select(x => new RecruitmentContactReport(x)).ToList();
            }
        }

        public string Client { get; set; }

        public int Code { get; set; }

        public JobSearchStatus Status { get; set; }

        public DateTime CreationDate { get; set; }

        public int ReasonId { get; set; }

        public string ReasonText { get; set; }

        public ReasonCauseType ReasonCauseType { get; set; }

        public int UserId { get; set; }

        public string UserText { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public string Seniorities { get; set; }

        public int Quantity { get; set; }

        public decimal MaximumSalary { get; set; }

        public IList<RecruitmentContactReport> Contacts { get; set; }
    }

    public class RecruitmentContactReport
    {
        public RecruitmentContactReport(JobSearchApplicant jobSearchApplicant)
        {
            if (jobSearchApplicant.Applicant != null)
            {
                Applicant = jobSearchApplicant.Applicant.LastName + " " + jobSearchApplicant.Applicant.FirstName;
            }

            if (jobSearchApplicant.Reason != null)
            {
                ReasonId = jobSearchApplicant.Reason.Id;
                ReasonText = jobSearchApplicant.Reason.Text;
                ReasonCauseType = jobSearchApplicant.Reason.Type;
            }

            if (jobSearchApplicant.RrhhInterviewer != null)
            {
                RrhhInterviewerId = jobSearchApplicant.RrhhInterviewerId;
                RrhhInterviewer = jobSearchApplicant.RrhhInterviewer.Name;
            }

            if (jobSearchApplicant.CreatedByUser != null)
            {
                RecruiterId = jobSearchApplicant.CreatedByUserId;
                RecruiterText = jobSearchApplicant.CreatedByUser.Name;
            }

            Date = jobSearchApplicant.CreatedDate;
            Comments = jobSearchApplicant.Comments;
            Salary = jobSearchApplicant.Salary;
            RemoteWork = jobSearchApplicant.RemoteWork;
            TechnicalInterviewer = jobSearchApplicant.TechnicalExternalInterviewer;
            HasRrhhInterview = jobSearchApplicant.HasRrhhInterview;
            HasTechnicalInterview = jobSearchApplicant.HasTechnicalInterview;
            HasClientInterview = jobSearchApplicant.HasClientInterview;

            RrhhInterviewComments = jobSearchApplicant.RrhhInterviewComments;
            TechnicalInterviewComments = jobSearchApplicant.TechnicalInterviewComments;
            ClientInterviewComments = jobSearchApplicant.ClientInterviewComments;
        }

        public int ReasonId { get; set; }

        public string RecruiterText { get; set; }

        public int? RecruiterId { get; set; }

        public int? RrhhInterviewerId { get; set; }

        public string ClientInterviewComments { get; set; }

        public string TechnicalInterviewComments { get; set; }

        public string RrhhInterviewComments { get; set; }

        public DateTime Date { get; set; }

        public bool HasClientInterview { get; set; }

        public bool HasTechnicalInterview { get; set; }

        public bool HasRrhhInterview { get; set; }

        public string Applicant { get; set; }

        public string ReasonText { get; set; }

        public ReasonCauseType ReasonCauseType { get; set; }

        public string Comments { get; set; }

        public decimal? Salary { get; set; }

        public bool RemoteWork { get; set; }

        public string RrhhInterviewer { get; set; }

        public string TechnicalInterviewer { get; set; }
    }
}
