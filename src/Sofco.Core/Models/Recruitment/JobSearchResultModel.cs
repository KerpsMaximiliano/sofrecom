using System;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchResultModel
    {
        public JobSearchResultModel(JobSearch jobSearch)
        {
            Id = jobSearch.Id;

            CreationDate = jobSearch.CreatedDate.Date;
            if (jobSearch.CloseDate.HasValue) CloseDate = jobSearch.CloseDate.Value.Date;
            if (jobSearch.ReopenDate.HasValue) ReopenDate = jobSearch.ReopenDate.Value.Date;
            if (jobSearch.SuspendedDate.HasValue) SuspendedDate = jobSearch.SuspendedDate.Value.Date;

            if (jobSearch.ReasonCause != null) Reason = jobSearch.ReasonCause.Text;
            if (jobSearch.Client != null) Client = jobSearch.Client.Name;
            if (jobSearch.User != null) User = jobSearch.User.Name;
            if (jobSearch.Recruiter != null) Recruiter = jobSearch.Recruiter.Name;

            Quantity = jobSearch.Quantity;
            MaxiumSalary = jobSearch.MaximunSalary;
            Comments = jobSearch.Comments;
            Status = jobSearch.Status;

            if (jobSearch.JobSearchProfiles != null && jobSearch.JobSearchProfiles.Any())
                Profiles = string.Join(";", jobSearch.JobSearchProfiles.Select(x => x.Profile.Text));

            if (jobSearch.JobSearchSeniorities != null && jobSearch.JobSearchSeniorities.Any())
                Seniorities = string.Join(";", jobSearch.JobSearchSeniorities.Select(x => x.Seniority.Text));

            if (jobSearch.JobSearchSkillsRequired != null && jobSearch.JobSearchSkillsRequired.Any())
                Skills = string.Join(";", jobSearch.JobSearchSkillsRequired.Select(x => x.Skill.Text));
        }

        public int Id { get; set; }

        public string Client { get; set; }

        public DateTime CreationDate { get; set; }

        public string Reason { get; set; }

        public string User { get; set; }

        public string Skills { get; set; }

        public string Profiles { get; set; }

        public string Seniorities { get; set; }

        public int Quantity { get; set; }

        public string TimeHiring { get; set; }

        public decimal MaxiumSalary { get; set; }

        public string Recruiter { get; set; }

        public DateTime? ReopenDate { get; set; }

        public DateTime? SuspendedDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public string Comments { get; set; }

        public JobSearchStatus Status { get; set; }
    }
}
