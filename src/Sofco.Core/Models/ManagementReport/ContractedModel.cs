using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class ContractedModel
    {
        public int ContractedId { get; set; }
        public int CostDetailId { get; set; }
        public string Name { get; set; }
        public decimal? Insurance { get; set; }
        public decimal? Honorary { get; set; }
        public decimal? Total { get; set; }
    }
}
