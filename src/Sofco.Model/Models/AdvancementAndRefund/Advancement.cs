﻿using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class Advancement : WorkflowEntity
    {
        public AdvancementPaymentForm PaymentForm { get; set; }

        public AdvancementType Type { get; set; }

        public int AdvancementReturnFormId { get; set; }
        public AdvancementReturnForm AdvancementReturnForm { get; set; }

        public DateTime StartDateReturn { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }
    }
}
