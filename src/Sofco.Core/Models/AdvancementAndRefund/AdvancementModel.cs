using System;
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

        public int? CurrencyId { get; set; }

        public string Description { get; set; }

        public decimal? Ammount { get; set; }

        public Advancement CreateDomain()
        {
            var domain = new Advancement();

            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.PaymentForm = PaymentForm.GetValueOrDefault();
            domain.Type = Type.GetValueOrDefault();
            domain.AdvancementReturnFormId = AdvancementReturnFormId.GetValueOrDefault();
            domain.StartDateReturn = StartDateReturn.GetValueOrDefault().Date;
            domain.CurrencyId = CurrencyId.GetValueOrDefault();
            domain.Description = Description;
            domain.Ammount = Ammount.GetValueOrDefault();

            return domain;
        }

        public void UpdateDomain(Advancement advancement)
        {
            advancement.UserApplicantId = UserApplicantId.GetValueOrDefault();
            advancement.PaymentForm = PaymentForm.GetValueOrDefault();
            advancement.Type = Type.GetValueOrDefault();
            advancement.AdvancementReturnFormId = AdvancementReturnFormId.GetValueOrDefault();
            advancement.StartDateReturn = StartDateReturn.GetValueOrDefault().Date;
            advancement.CurrencyId = CurrencyId.GetValueOrDefault();
            advancement.Description = Description;
            advancement.Ammount = Ammount.GetValueOrDefault();
        }
    }
}
