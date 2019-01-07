using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models
{
    public class EmployeeAdvancementDetail
    {
        public EmployeeAdvancementDetail(Advancement advancement)
        {
            Id = advancement.Id;

            PaymentForm = advancement.PaymentForm;
            Type = advancement.Type;
            CurrencyDesc = advancement.Currency?.Text;
            StatusDesc = advancement.Status?.Name;
            CreationDate = advancement.CreationDate;
            Ammount = advancement.Ammount;
            WorkflowStatusType = advancement.Status?.Type;
        }

        public int Id { get; set; }

        public AdvancementType Type { get; set; }

        public string CurrencyDesc { get; set; }

        public AdvancementPaymentForm PaymentForm { get; set; }

        public string StatusDesc { get; set; }

        public decimal Ammount { get; set; }

        public DateTime CreationDate { get; set; }

        public WorkflowStateType? WorkflowStatusType { get; set; }
    }
}
