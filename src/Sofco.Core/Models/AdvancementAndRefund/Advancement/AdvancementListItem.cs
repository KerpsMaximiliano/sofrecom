﻿using System;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.AdvancementAndRefund.Advancement
{
    public class AdvancementListItem
    {
        public AdvancementListItem(Domain.Models.AdvancementAndRefund.Advancement advancement)
        {
            Id = advancement.Id;

            UserApplicantId = advancement.UserApplicantId;
            UserApplicantDesc = advancement.UserApplicant?.Name;
            Type = advancement.Type;
            CurrencyDesc = advancement.Currency?.Text;
            StatusDesc = advancement.Status?.Name;
            CreationDate = advancement.CreationDate;
            Ammount = advancement.Ammount;
            WorkflowStatusType = advancement.Status?.Type;
            MonthsReturn = advancement.MonthsReturn?.Text;
            PaymentForm = advancement.PaymentForm;
            Description = advancement.Description;
            WorkflowId = advancement.WorkflowId;
        }

        public string Description { get; set; }

        public AdvancementPaymentForm PaymentForm { get; set; }

        public string MonthsReturn { get; set; }

        public int Id { get; set; }

        public AdvancementType Type { get; set; }

        public string CurrencyDesc { get; set; }

        public int UserApplicantId { get; set; }

        public string UserApplicantDesc { get; set; }

        public string StatusDesc { get; set; }

        public string Bank { get; set; }

        public decimal Ammount { get; set; }

        public DateTime CreationDate { get; set; }

        public WorkflowStateType? WorkflowStatusType { get; set; }

        public int WorkflowId { get; set; }

        public int NextWorkflowStateId { get; set; }
    }
}
