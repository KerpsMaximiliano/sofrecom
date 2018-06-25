using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class PurchaseOrderEditModel
    {
        public PurchaseOrderEditModel()
        {
        }

        public PurchaseOrderEditModel(PurchaseOrder domain)
        {
            Id = domain.Id;
            Number = domain.Number;
            ClientExternalId = domain.ClientExternalId;
            ClientExternalName = domain.ClientExternalName;
            ReceptionDate = domain.ReceptionDate;
            AreaId = domain.AreaId.GetValueOrDefault();
            Status = domain.Status;
            StartDate = domain.StartDate;
            EndDate = domain.EndDate;
            ReceptionDate = domain.ReceptionDate;
            Description = domain.Description;

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

            if (domain.AmmountDetails != null)
            {
                AmmountDetails = domain.AmmountDetails.Select(x => new PurchaseOrderAmmountDetailModel
                    {
                        CurrencyId = x.CurrencyId,
                        Ammount = x.Ammount,
                        Enable = false,
                        CurrencyDescription = x.Currency.Text
                    })
                    .ToList();
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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ReceptionDate { get; set; }

        public int AreaId { get; set; }

        public string Description { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public int[] AnalyticIds { get; set; }

        public IList<PurchaseOrderAmmountDetailModel> AmmountDetails { get; set; }
    }
}
