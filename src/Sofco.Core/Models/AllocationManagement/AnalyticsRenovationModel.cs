using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace Sofco.Core.Models.AllocationManagement
{
    public class AnalyticsRenovationModel : BaseEntity
    {
        public AnalyticsRenovationModel()
        {

        }

        public AnalyticsRenovationModel(AnalyticsRenovation domain)
        {
            Id = domain.Id;
            Renovation = domain.Renovation;
            AnalyticId = domain.AnalyticId;
            StartDate = domain.StartDate;
            EndDate = domain.EndDate;
            Orden = domain.Orden;
            OpportunityId = domain.OpportunityId;
            ModifiedAt = domain.ModifiedAt;
            ModifiedBy = domain.ModifiedBy;
            CreatedAt = domain.CreatedAt;
            CreatedBy = domain.CreatedBy;
        }

        public int Renovation { get; set; }

        public int AnalyticId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Orden { get; set; }

        public int OpportunityId { get; set; }

        public DateTime ModifiedAt { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public virtual AnalyticsRenovation CreateDomain()
        {
            var domain = new AnalyticsRenovation();

            FillData(domain);
                        
            domain.CreatedAt = DateTime.UtcNow;

            return domain;
        }

        public void UpdateDomain(AnalyticsRenovation renovation)
        {
            FillData(renovation);
        }

        protected void FillData(AnalyticsRenovation domain)
        {
            domain.Id = Id;
            domain.Renovation = Renovation;
            domain.AnalyticId = AnalyticId;
            domain.StartDate = StartDate;
            domain.EndDate = EndDate;
            domain.Orden = Orden;
            domain.OpportunityId = OpportunityId;
            domain.ModifiedAt = ModifiedAt;
            domain.ModifiedBy = ModifiedBy;
            domain.CreatedAt = DateTime.UtcNow;
            domain.CreatedBy = CreatedBy;

        }
    }
}
