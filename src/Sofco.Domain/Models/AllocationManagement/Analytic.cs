using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class Analytic : BaseEntity
    {
        public string Title { get; set; }

        public int TitleId { get; set; }

        public int CostCenterId { get; set; }

        public CostCenter CostCenter { get; set; }

        public string Name { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string ServiceName { get; set; }

        public string ServiceId { get; set; }

        public int? ActivityId { get; set; }
        public ImputationNumber Activity { get; set; }

        public DateTime StartDateContract { get; set; }

        public DateTime EndDateContract { get; set; }

        public int SectorId { get; set; }
        public Sector Sector { get; set; }

        public int? ManagerId { get; set; }
        public User Manager { get; set; }

        public int? CommercialManagerId { get; set; }
        public User CommercialManager { get; set; }

        public string Proposal { get; set; }

        public AnalyticStatus Status { get; set; }

        public int? SolutionId { get; set; }
        public Solution Solution { get; set; }

        public int? TechnologyId { get; set; }
        public Technology Technology { get; set; }

        public DateTime CreationDate { get; set; }

        public int? ClientGroupId { get; set; }
        public ClientGroup ClientGroup { get; set; }

        public int? ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }

        public string UsersQv { get; set; }

        public int? SoftwareLawId { get; set; }
        
        public SoftwareLaw SoftwareLaw { get; set; }

        public ICollection<Allocation> Allocations { get; set; }

        public ICollection<WorkTime> WorkTimes { get; set; }

        public ICollection<PurchaseOrderAnalytic> PurchaseOrderAnalytics { get; set; }

        public ICollection<Refund> Refunds { get; set; }

        public ICollection<CostDetail> CostDetail { get; set; }
    }
}
