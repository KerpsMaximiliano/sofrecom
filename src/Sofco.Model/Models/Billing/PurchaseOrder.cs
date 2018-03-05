using System;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Common;

namespace Sofco.Model.Models.Billing
{
    public class PurchaseOrder : BaseEntity
    {
        public string Title { get; set; }

        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int ManagerId { get; set; }
        public User Manager { get; set; }

        public int CommercialManagerId { get; set; }
        public User CommercialManager { get; set; }

        public int AnalyticId { get; set; }
        public Analytic Analytic { get; set; }

        public DateTime ReceptionDate { get; set; }

        public string Area { get; set; }

        public int Year { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateByUser { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }
    }
}
