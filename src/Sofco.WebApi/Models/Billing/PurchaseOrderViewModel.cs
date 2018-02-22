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

    public class PurchaseOrderEditViewModel
    {
        public PurchaseOrderEditViewModel()
        {
        }

        public PurchaseOrderEditViewModel(PurchaseOrder domain)
        {
            Id = domain.Id;
            Title = domain.Title;
            Number = domain.Number;
            ClientExternalId = domain.ClientExternalId;
            ClientExternalName = domain.ClientExternalName;
            ManagerId = domain.ManagerId;
            CommercialManagerId = domain.CommercialManagerId;
            AnalyticId = domain.AnalyticId;
            ReceptionDate = domain.ReceptionDate;
            Area = domain.Area;
            Year = domain.Year;
            Status = domain.Status;

            if (domain.File != null)
            {
                FileId = domain.FileId.GetValueOrDefault();
                FileName = domain.File.FileName;
                CreationDate = domain.File.CreationDate.ToString("d");
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int ManagerId { get; set; }

        public int CommercialManagerId { get; set; }

        public int AnalyticId { get; set; }

        public DateTime ReceptionDate { get; set; }

        public string Area { get; set; }

        public int Year { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public PurchaseOrder CreateDomain(string userName)
        {
            var domain = new PurchaseOrder();

            domain.Id = Id;
            domain.Title = Title;
            domain.Number = Number;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.ManagerId = ManagerId;
            domain.CommercialManagerId = CommercialManagerId;
            domain.AnalyticId = AnalyticId;
            domain.ReceptionDate = ReceptionDate;
            domain.Area = Area;
            domain.Year = Year;
            domain.Status = Status;

            if (FileId > 0) domain.FileId = FileId;

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;

            return domain;
        }
    }
}
