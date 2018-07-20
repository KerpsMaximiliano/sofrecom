using System;
using System.Collections.Generic;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;

namespace Sofco.Model.Models.Billing
{
    public class PurchaseOrder : BaseEntity
    {
        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public DateTime ReceptionDate { get; set; }

        public int? AreaId { get; set; }
        public Area Area { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateByUser { get; set; }
         
        public int? FileId { get; set; }
        public File File { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public bool Adjustment { get; set; }

        public string FicheDeSignature { get; set; }

        public string PaymentForm { get; set; }

        public decimal Margin { get; set; }

        public string Comments { get; set; }

        public ICollection<PurchaseOrderAnalytic> PurchaseOrderAnalytics { get; set; }

        public ICollection<Solfac> Solfacs { get; set; }

        public ICollection<PurchaseOrderAmmountDetail> AmmountDetails { get; set; }

        public ICollection<PurchaseOrderHistory> Histories { get; set; }
    }
}
