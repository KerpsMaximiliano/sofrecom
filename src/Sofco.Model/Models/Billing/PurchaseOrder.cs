using System;
using System.Collections.Generic;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Relationships;

namespace Sofco.Model.Models.Billing
{
    public class PurchaseOrder : BaseEntity
    {
        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public DateTime ReceptionDate { get; set; }

        public string Area { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateByUser { get; set; }
         
        public int? FileId { get; set; }
        public File File { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public ICollection<PurchaseOrderAnalytic> PurchaseOrderAnalytics { get; set; }

        public ICollection<Solfac> Solfacs { get; set; }

        public ICollection<PurchaseOrderAmmountDetail> AmmountDetails { get; set; }
    }
}
