using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing.PurchaseOrder
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

        public int AreaId { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string FicheDeSignature { get; set; }

        public string PaymentForm { get; set; }

        public decimal Margin { get; set; }

        public string Comments { get; set; }

        public IList<PurchaseOrderAmmountDetailModel> AmmountDetails { get; set; }

        public Model.Models.Billing.PurchaseOrder CreateDomain(string userName)
        {
            var domain = new Model.Models.Billing.PurchaseOrder();

            FillData(domain, userName);

            domain.Status = PurchaseOrderStatus.Draft;
       
            domain.Histories = new List<PurchaseOrderHistory>();

            domain.AmmountDetails = AmmountDetails.Where(x => x.Enable).Select(x => new PurchaseOrderAmmountDetail
            {
                CurrencyId = x.CurrencyId,
                Balance = x.Ammount,
                Ammount = x.Ammount
            })
            .ToList();

            return domain;
        }

        public void UpdateDomain(Model.Models.Billing.PurchaseOrder domain, string userName)
        {
            FillData(domain, userName);

            domain.Status = Status;

            if (FileId > 0)
                domain.FileId = FileId;

            foreach (var detail in AmmountDetails)
            {
                var domainDetail = domain.AmmountDetails.SingleOrDefault(x => x.CurrencyId == detail.CurrencyId);

                if (domainDetail != null)
                {
                    if (detail.Enable)
                    {
                        domainDetail.Balance = detail.Balance;
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
                        Balance = detail.Ammount,
                        Ammount = detail.Ammount
                    });
                }
            }
        }

        private void FillData(Model.Models.Billing.PurchaseOrder domain, string userName)
        {
            domain.Number = Number;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.StartDate = StartDate;
            domain.EndDate = EndDate;
            domain.ReceptionDate = ReceptionDate;
            domain.AreaId = AreaId;
            domain.Description = Description;
            domain.FicheDeSignature = FicheDeSignature;
            domain.PaymentForm = PaymentForm;
            domain.Margin = Margin;
            domain.Comments = Comments;

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;
        }
    }
}
