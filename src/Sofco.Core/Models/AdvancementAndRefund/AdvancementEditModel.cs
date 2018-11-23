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

            CurrencyId = advancement.CurrencyId;
            CurrencyDesc = advancement.Currency?.Text;

            StatusId = advancement.StatusId;
            StatusDesc = advancement.Status?.Name;
            WorkflowStateType = advancement.Status?.Type;

            Description = advancement.Description;
            Ammount = advancement.Ammount;
        }

        public int Id { get; set; }

        public int UserApplicantId { get; set; }

        public AdvancementPaymentForm PaymentForm { get; set; }

        public AdvancementType Type { get; set; }

        public WorkflowStateType? WorkflowStateType { get; set; }

        public int AdvancementReturnFormId { get; set; }

        public DateTime StartDateReturn { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyDesc { get; set; }

        public string AdvancementReturnFormDesc { get; set; }

        public string UserApplicantDesc { get; set; }

        public string StatusDesc { get; set; }

        public int StatusId { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }
    }
}
