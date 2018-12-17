﻿using System;
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

            MonthsReturnId = advancement.MonthsReturnId;
            MonthsReturnDesc = advancement.MonthsReturn?.Text;

            AdvancementReturnForm = advancement.AdvancementReturnForm;
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

        public int? MonthsReturnId { get; set; }

        public DateTime? StartDateReturn { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyDesc { get; set; }

        public string AdvancementReturnForm { get; set; }

        public string UserApplicantDesc { get; set; }

        public string StatusDesc { get; set; }

        public int StatusId { get; set; }

        public string Description { get; set; }

        public string MonthsReturnDesc { get; set; }

        public decimal Ammount { get; set; }
    }
}
