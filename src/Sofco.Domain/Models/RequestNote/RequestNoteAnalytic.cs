using System;
using System.Collections.Generic;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteAnalytic : BaseEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public int AnalyticId { get; set; }
        public Analytic Analytic { get; set; }
        public int Percentage { get; set; }
        public string Status { get; set; }
    }
}
