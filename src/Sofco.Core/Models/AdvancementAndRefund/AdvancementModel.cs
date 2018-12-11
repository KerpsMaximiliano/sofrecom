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

        public int? MonthsReturnId { get; set; }

        public DateTime? StartDateReturn { get; set; }

        public int? CurrencyId { get; set; }

        public string Description { get; set; }

        public string AdvancementReturnForm { get; set; }

        public decimal? Ammount { get; set; }

        public Advancement CreateDomain()
        {
            var domain = new Advancement();

            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.PaymentForm = PaymentForm.GetValueOrDefault();
            domain.Type = Type.GetValueOrDefault();
            domain.CurrencyId = CurrencyId.GetValueOrDefault();
            domain.Description = Description;
            domain.Ammount = Ammount.GetValueOrDefault();

            if (domain.Type == AdvancementType.Salary)
            {
                domain.MonthsReturnId = MonthsReturnId.GetValueOrDefault();
                domain.AdvancementReturnForm = AdvancementReturnForm;
            }

            if (domain.Type == AdvancementType.Viaticum)
            {
                domain.StartDateReturn = StartDateReturn.GetValueOrDefault().Date;
            }

            return domain;
        }

        public void UpdateDomain(Advancement domain)
        {
            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.PaymentForm = PaymentForm.GetValueOrDefault();
            domain.Type = Type.GetValueOrDefault();
            domain.CurrencyId = CurrencyId.GetValueOrDefault();
            domain.Description = Description;
            domain.Ammount = Ammount.GetValueOrDefault();

            if (domain.Type == AdvancementType.Salary)
            {
                domain.MonthsReturnId = MonthsReturnId.GetValueOrDefault();
                domain.AdvancementReturnForm = AdvancementReturnForm;
            }

            if (domain.Type == AdvancementType.Viaticum)
            {
                domain.StartDateReturn = StartDateReturn.GetValueOrDefault().Date;
            }
        }
    }
}
