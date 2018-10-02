using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderEditModel
    {
        public PurchaseOrderEditModel()
        {
        }

        public PurchaseOrderEditModel(Domain.Models.Billing.PurchaseOrder domain)
        {
            Id = domain.Id;
            Number = domain.Number;
            Title = domain.Title;
            ClientExternalId = domain.ClientExternalId;
            ClientExternalName = domain.ClientExternalName;
            ReceptionDate = domain.ReceptionDate;
            AreaId = domain.AreaId.GetValueOrDefault();
            Status = domain.Status;
            StartDate = domain.StartDate;
            EndDate = domain.EndDate;
            ReceptionDate = domain.ReceptionDate;
            Description = domain.Description;
            FicheDeSignature = domain.FicheDeSignature;
            PaymentForm = domain.PaymentForm;
            Margin = domain.Margin;
            Comments = domain.Comments;
            Proposal = domain.Proposal;

            if (!string.IsNullOrWhiteSpace(domain.Proposal))
            {
                ProposalIds = domain.Proposal.Split(';');
            }

            if (domain.PurchaseOrderAnalytics.Any())
            {
                AnalyticIds = domain.PurchaseOrderAnalytics.Select(x => x.AnalyticId).ToArray();
                SectorIds = domain.PurchaseOrderAnalytics.Where(x => x.Analytic != null).Select(x => x.Analytic.SectorId).ToArray();
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
                        Enable = true,
                        CurrencyDescription = x.Currency.Text
                    })
                    .ToList();
            }
        }

        public string Title { get; set; }

        public string Proposal { get; set; }

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

        public string FicheDeSignature { get; set; }

        public string PaymentForm { get; set; }

        public decimal Margin { get; set; }

        public string Comments { get; set; }

        public int[] AnalyticIds { get; set; }

        public string[] ProposalIds { get; set; }

        public IList<PurchaseOrderAmmountDetailModel> AmmountDetails { get; set; }

        public int[] SectorIds { get; set; }
    }
}
