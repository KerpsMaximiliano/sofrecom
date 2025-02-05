﻿using System;
using System.Collections.Generic;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Recruitment
{
    public class JobSearchApplicant
    {
        public int JobSearchId { get; set; }

        public JobSearch JobSearch { get; set; }

        public int ApplicantId { get; set; }

        public Applicant Applicant { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ReasonId { get; set; }

        public ReasonCause Reason { get; set; }

        public string Comments { get; set; }

        public string CreatedBy { get; set; }
        public int? CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public bool HasRrhhInterview { get; set; }

        public DateTime? RrhhInterviewDate { get; set; }

        public string RrhhInterviewPlace { get; set; }

        public int? RrhhInterviewerId { get; set; }

        public User RrhhInterviewer { get; set; }

        public bool HasTechnicalInterview { get; set; }

        public DateTime? TechnicalInterviewDate { get; set; }

        public string TechnicalInterviewPlace { get; set; }

        public bool HasClientInterview { get; set; }

        public DateTime? ClientInterviewDate { get; set; }

        public string ClientInterviewPlace { get; set; }

        public string TechnicalInterviewComments { get; set; }

        public string TechnicalExternalInterviewer { get; set; }

        public string ClientInterviewComments { get; set; }

        public string ClientExternalInterviewer { get; set; }

        public string RrhhInterviewComments { get; set; }

        public bool RemoteWork { get; set; }

        public decimal? Salary { get; set; }

        public DateTime ModifiedAt { get; set; }

        public bool HasPhoneInterview { get; set; }
        public string PhoneInterviewComments { get; set; }
    }
}
