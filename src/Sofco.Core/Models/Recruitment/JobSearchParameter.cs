﻿using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchParameter
    {
        public int? Id { get; set; }

        public string ClientId { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> Seniorities { get; set; }

        public int? UserId { get; set; }

        public IList<JobSearchStatus> Status { get; set; }

        public int? ReasonId { get; set; }

        public int? RecruiterId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
