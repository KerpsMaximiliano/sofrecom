﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundModel
    {
        public int Id { get; set; }

        public int? UserApplicantId { get; set; }

        public int? CurrencyId { get; set; }

        public int? AnalyticId { get; set; }

        public int? CreditCardId { get; set; }

        public bool HasCreditCard { get; set; }

        public bool LastRefund { get; set; }

        public IList<int> Advancements { get; set; }

        public IList<RefundDetailModel> Details { get; set; }

        public Domain.Models.AdvancementAndRefund.Refund CreateDomain()
        {
            var domain = new Domain.Models.AdvancementAndRefund.Refund();

            domain.Details = new List<RefundDetail>();

            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.CurrencyId = CurrencyId.GetValueOrDefault();
            domain.AnalyticId = AnalyticId.GetValueOrDefault();
            domain.CreationDate = DateTime.UtcNow;
            domain.TotalAmmount = Details.Sum(x => x.Ammount);
            domain.InWorkflowProcess = true;
            domain.LastRefund = LastRefund;
            domain.CashReturn = CashReturn;

            if (HasCreditCard)
                domain.CreditCardId = CreditCardId;

            foreach (var detail in Details)
            {
                domain.Details.Add(detail.CreateDomain());
            }

            return domain;
        }

        public bool CashReturn { get; set; }

        public void UpdateDomain(Domain.Models.AdvancementAndRefund.Refund domain)
        {
            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.CurrencyId = CurrencyId.GetValueOrDefault();
            domain.AnalyticId = AnalyticId.GetValueOrDefault();
            domain.TotalAmmount = Details.Sum(x => x.Ammount);
            domain.LastRefund = LastRefund;
            domain.CashReturn = CashReturn;

            if (HasCreditCard)
                domain.CreditCardId = CreditCardId;

            foreach (var detail in Details)
            {
                if (detail.Id == 0)
                {
                    domain.Details.Add(detail.CreateDomain());
                }
                else
                {
                    var domainDetail = domain.Details.SingleOrDefault(x => x.Id == detail.Id);

                    if (domainDetail != null)
                    {
                        domainDetail.Order = detail.Order;
                        domainDetail.Description = detail.Description;
                        domainDetail.Ammount = detail.Ammount;
                        domainDetail.CreationDate = detail.CreationDate.GetValueOrDefault();
                        domainDetail.CostTypeId = detail.CostTypeId;
                    }
                }
            }

            var detailsIds = Details.Select(x => x.Id).ToList();

            if (detailsIds.Any())
            {
                var detailsToRemove = domain.Details.Where(x => !detailsIds.Contains(x.Id)).ToList();

                foreach (var refundDetail in detailsToRemove)
                {
                    domain.Details.Remove(refundDetail);
                }
            }
        }
    }

    public class RefundDetailModel
    {
        public int Id { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public int Order { get; set; }

        public int? CostTypeId { get; set; }

        public RefundDetail CreateDomain()
        {
            var domain = new RefundDetail();

            domain.Description = Description;
            domain.Ammount = Ammount;
            domain.CreationDate = CreationDate.GetValueOrDefault();
            domain.Order = Order;
            domain.CostTypeId = CostTypeId;

            return domain;
        }
    }
}
