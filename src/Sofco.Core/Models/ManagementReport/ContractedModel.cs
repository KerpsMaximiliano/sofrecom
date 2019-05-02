using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class ContractedModel
    {
        public int ContractedId { get; set; }
        public int AnalyticId { get; set; }
        public string Name { get; set; }
        public DateTime MonthYear { get; set; }
        public float? insurance { get; set; }
        public float? honorary { get; set; }
        public float? total { get; set; }
    }
}
