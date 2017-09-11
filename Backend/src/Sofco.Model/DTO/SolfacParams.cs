﻿using Sofco.Model.Enums;

namespace Sofco.Model.DTO
{
    public class SolfacParams
    {
        public string CustomerId { get; set; }
        public string ServiceId { get; set; }
        public string ProjectId { get; set; }
        public string Analytic { get; set; }
        public SolfacStatus Status { get; set; }
        public int UserApplicantId { get; set; }
    }
}
