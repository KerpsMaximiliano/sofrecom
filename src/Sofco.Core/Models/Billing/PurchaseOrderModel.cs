using System;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class PurchaseOrderModel
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int[] AnalyticIds { get; set; }

        public int CurrencyId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ReceptionDate { get; set; }

        public string Area { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public PurchaseOrder CreateDomain(string userName)
        {
            var domain = new PurchaseOrder();

            domain.Number = Number;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.CurrencyId = CurrencyId;
            domain.StartDate = StartDate;
            domain.EndDate = EndDate;
            domain.ReceptionDate = ReceptionDate;
            domain.Area = Area;
            domain.Description = Description;
            domain.Ammount = Ammount;

            domain.Status = PurchaseOrderStatus.Valid;
            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;

            return domain;
        }

        public void UpdateDomain(PurchaseOrder domain, string userName)
        {
            FillData(domain);

            domain.Status = Status;

            if (FileId > 0)
                domain.FileId = FileId;

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;
        }

        private void FillData(PurchaseOrder domain)
        {
            domain.Number = Number;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.CurrencyId = CurrencyId;
            domain.StartDate = StartDate;
            domain.EndDate = EndDate;
            domain.ReceptionDate = ReceptionDate;
            domain.Area = Area;
            domain.Description = Description;
            domain.Ammount = Ammount;
        }
    }
}
