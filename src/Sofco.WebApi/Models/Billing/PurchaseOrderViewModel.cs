using System;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class PurchaseOrderViewModel
    {
        public string Title { get; set; }

        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int ManagerId { get; set; }

        public int CommercialManagerId { get; set; }

        public string ProjectId { get; set; }

        public int AnalyticId { get; set; }

        public DateTime ReceptionDate { get; set; }

        public string Area { get; set; }

        public int Year { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public PurchaseOrder CreateDomain(string userName)
        {
            var domain = new PurchaseOrder();

            domain.Title = Title;
            domain.Number = Number;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.ProjectId = ProjectId;
            domain.ManagerId = ManagerId;
            domain.CommercialManagerId = CommercialManagerId;
            domain.AnalyticId = AnalyticId;
            domain.ReceptionDate = ReceptionDate;
            domain.Area = Area;
            domain.Year = Year;

            domain.Status = PurchaseOrderStatus.Valid;
            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;

            return domain;
        }
    }
}
