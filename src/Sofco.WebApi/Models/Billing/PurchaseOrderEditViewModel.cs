using System;
using System.Linq;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class PurchaseOrderEditViewModel
    {
        public PurchaseOrderEditViewModel()
        {
        }

        public PurchaseOrderEditViewModel(PurchaseOrder domain)
        {
            Id = domain.Id;
            Number = domain.Number;
            ClientExternalId = domain.ClientExternalId;
            ClientExternalName = domain.ClientExternalName;
            ReceptionDate = domain.ReceptionDate;
            Area = domain.Area;
            Status = domain.Status;
            CurrencyId = domain.CurrencyId;
            StartDate = domain.StartDate;
            EndDate = domain.EndDate;
            ReceptionDate = domain.ReceptionDate;
            Description = domain.Description;
            Ammount = domain.Ammount;

            if (domain.PurchaseOrderAnalytics.Any())
            {
                AnalyticIds = domain.PurchaseOrderAnalytics.Select(x => x.AnalyticId).ToArray();
            }

            if (domain.File != null)
            {
                FileId = domain.FileId.GetValueOrDefault();
                FileName = domain.File.FileName;
                CreationDate = domain.File.CreationDate.ToString("d");
            }
        }

        public int Id { get; set; }

        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public string ProjectId { get; set; }

        public string OpportunityId { get; set; }

        public string OpportunityDescription { get; set; }

        public int AnalyticId { get; set; }

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

        public string CreationDate { get; set; }

        public int[] AnalyticIds { get; set; }
    }
}
