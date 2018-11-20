using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models.AdvancementAndRefund
{
    public class AdvancementEditModel
    {
        public AdvancementEditModel(Advancement advancement)
        {
            Id = advancement.Id;

            UserApplicantId = advancement.UserApplicantId;
            UserApplicantDesc = advancement.UserApplicant?.Name;

            PaymentForm = advancement.PaymentForm;
            Type = advancement.Type;

            AdvancementReturnFormId = advancement.AdvancementReturnFormId;
            AdvancementReturnFormDesc = advancement.AdvancementReturnForm?.Text;

            StartDateReturn = advancement.StartDateReturn;

            AnalyticId = advancement.AnalyticId;
            AnalyticDesc = $"{advancement.Analytic?.Title}-{advancement.Analytic?.Name}";

            CurrencyId = advancement.CurrencyId;
            CurrencyDesc = advancement.Currency?.Text;

            Details = new List<AdvancementDetailModel>();

            foreach (var detail in advancement.Details)
            {
                var item = new AdvancementDetailModel();
                item.Date = detail.Date;
                item.Ammount = detail.Ammount;
                item.Description = detail.Description;

                Details.Add(item);
            }
        }

        public int Id { get; set; }

        public int UserApplicantId { get; set; }

        public AdvancementPaymentForm PaymentForm { get; set; }

        public AdvancementType Type { get; set; }

        public int AdvancementReturnFormId { get; set; }

        public DateTime StartDateReturn { get; set; }

        public int AnalyticId { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyDesc { get; set; }

        public string AnalyticDesc { get; set; }

        public string AdvancementReturnFormDesc { get; set; }

        public string UserApplicantDesc { get; set; }

        public IList<AdvancementDetailModel> Details { get; set; }
    }
}
