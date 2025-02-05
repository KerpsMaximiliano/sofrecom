﻿using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundListParameterModel
    {
        public int? UserApplicantId { get; set; }

        public DateTime? DateSince { get; set; }

        public DateTime? DateTo { get; set; }

        public int[] StateIds { get; set; }

        public int[] AnalyticIds { get; set; }

        public bool InWorkflowProcess { get; set; }

        public List<int> UserApplicantIds { get; set; }

        public string Bank { get; set; }
    }
}
