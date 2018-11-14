using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models.AdvancementAndRefund
{
    public class AdvancementModel
    {
        public int Id { get; set; }

        public int? UserApplicantId { get; set; }

        public AdvancementPaymentForm? PaymentForm { get; set; }

        public AdvancementType? Type { get; set; }

        public int? AdvancementReturnFormId { get; set; }

        public DateTime? StartDateReturn { get; set; }

        public int? AnalyticId { get; set; }

        public int? CurrencyId { get; set; }

        public IList<AdvancementDetailModel> Details { get; set; }

        public Advancement CreateDomain()
        {
            var domain = new Advancement();

            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.PaymentForm = PaymentForm.GetValueOrDefault();
            domain.Type = Type.GetValueOrDefault();
            domain.AdvancementReturnFormId = AdvancementReturnFormId.GetValueOrDefault();
            domain.StartDateReturn = StartDateReturn.GetValueOrDefault().Date;
            domain.AnalyticId = AnalyticId.GetValueOrDefault();
            domain.CurrencyId = CurrencyId.GetValueOrDefault();

            domain.Details = new List<AdvancementDetail>();

            foreach (var detail in Details)
            {
                var item = new AdvancementDetail();
                item.Date = detail.Date.GetValueOrDefault().Date;
                item.Description = detail.Description;
                item.Ammount = detail.Ammount.GetValueOrDefault();

                domain.Details.Add(item);
            }

            return domain;
        }

        public void UpdateDomain(Advancement advancement)
        {
            advancement.UserApplicantId = UserApplicantId.GetValueOrDefault();
            advancement.PaymentForm = PaymentForm.GetValueOrDefault();
            advancement.Type = Type.GetValueOrDefault();
            advancement.AdvancementReturnFormId = AdvancementReturnFormId.GetValueOrDefault();
            advancement.StartDateReturn = StartDateReturn.GetValueOrDefault().Date;
            advancement.AnalyticId = AnalyticId.GetValueOrDefault();
            advancement.CurrencyId = CurrencyId.GetValueOrDefault();

            if (advancement.Type == AdvancementType.Salary)
            {
                var detail = Details.First();
                var detailDomain = advancement.Details.First();

                if (detail != null && detailDomain != null)
                {
                    detailDomain.Date = detail.Date.GetValueOrDefault();
                    detailDomain.Description = detail.Description;
                    detailDomain.Ammount = detail.Ammount.GetValueOrDefault();
                }
            }
        }
    }

    public class AdvancementDetailModel
    {
        public DateTime? Date { get; set; }

        public string Description { get; set; }

        public decimal? Ammount { get; set; }
    }
}
