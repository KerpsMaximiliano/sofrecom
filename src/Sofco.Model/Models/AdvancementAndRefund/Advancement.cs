using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class Advancement : BaseEntity
    {
        public int UserApplicantId { get; set; }
        public User UserApplicant { get; set; }

        public AdvancementPaymentForm PaymentForm { get; set; }

        public AdvancementType Type { get; set; }

        public int AdvancementReturnFormId { get; set; }
        public AdvancementReturnForm AdvancementReturnForm { get; set; }

        public DateTime StartDateReturn { get; set; }

        public int AnalyticId { get; set; }
        public Analytic Analytic { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public IList<AdvancementDetail> Details { get; set; }
    }
}
