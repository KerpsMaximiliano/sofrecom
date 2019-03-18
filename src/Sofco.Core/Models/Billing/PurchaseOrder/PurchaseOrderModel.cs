using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Number { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int[] AnalyticIds { get; set; }

        public string[] ProposalIds { get; set; }

        public int CurrencyId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ReceptionDate { get; set; }

        public int AreaId { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string FicheDeSignature { get; set; }

        public string PaymentForm { get; set; }

        public decimal? Margin { get; set; }

        public string Comments { get; set; }


        public IList<PurchaseOrderAmmountDetailModel> AmmountDetails { get; set; }

        public Domain.Models.Billing.PurchaseOrder CreateDomain(string userName)
        {
            var domain = new Domain.Models.Billing.PurchaseOrder();

            domain.Status = PurchaseOrderStatus.Draft;

            FillData(domain, userName);

            domain.Histories = new List<PurchaseOrderHistory>();

            domain.AmmountDetails = AmmountDetails.Where(x => x.Enable).Select(x => new PurchaseOrderAmmountDetail
            {
                CurrencyId = x.CurrencyId,
                Ammount = x.Ammount
            })
            .ToList();

            return domain;
        }

        public void UpdateDomain(Domain.Models.Billing.PurchaseOrder domain, string userName)
        {
            FillData(domain, userName);

            foreach (var detail in AmmountDetails)
            {
                var domainDetail = domain.AmmountDetails.SingleOrDefault(x => x.CurrencyId == detail.CurrencyId);

                if (domainDetail != null)
                {
                    if (detail.Enable)
                    {
                        domainDetail.Ammount = detail.Ammount;
                    }
                    else
                    {
                        domain.AmmountDetails.Remove(domainDetail);
                    }
                }
                else
                {
                    if(!detail.Enable) continue;

                    domain.AmmountDetails.Add(new PurchaseOrderAmmountDetail
                    {
                        CurrencyId = detail.CurrencyId,
                        Ammount = detail.Ammount
                    });
                }
            }
        }

        private void FillData(Domain.Models.Billing.PurchaseOrder domain, string userName)
        {
            domain.Title = Title;
            domain.Number = Number;
            domain.StartDate = StartDate;
            domain.EndDate = EndDate;
            domain.ReceptionDate = ReceptionDate;
            domain.Description = Description;
            domain.FicheDeSignature = FicheDeSignature;
            domain.PaymentForm = PaymentForm;
            domain.Comments = Comments;
            domain.Proposal = string.Join(";", ProposalIds);

            if (domain.Status == PurchaseOrderStatus.Draft || domain.Status == PurchaseOrderStatus.Reject)
            {
                domain.AreaId = AreaId;
                domain.Margin = Margin.GetValueOrDefault();
                domain.AccountId = ClientExternalId;
                domain.AccountName = ClientExternalName;
            }

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;
        }
    }
}
