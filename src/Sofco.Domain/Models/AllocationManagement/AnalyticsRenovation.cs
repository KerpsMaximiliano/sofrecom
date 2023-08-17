using Sofco.Domain.Models.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class AnalyticsRenovation : BaseEntity
    {
        public int Renovation{ get; set; }
        
        public int AnalyticId{ get; set; }

        public Analytic Analytic { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public int Orden { get; set; }
        
        public int OpportunityId { get; set; }

        public Opportunity Opportunity { get; set; }
        
        public DateTime ModifiedAt { get; set; }
        
        public string ModifiedBy{ get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string CreatedBy { get; set; }
    }
}
