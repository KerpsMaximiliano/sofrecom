using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Models.AdvancementAndRefund.Common
{
    public class PaymentPendingModel
    {
        public int Id { get; set; }

        public string Bank { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyDesc { get; set; }

        public int UserApplicantId { get; set; }

        public string UserApplicantDesc { get; set; }

        public decimal Ammount { get; set; }

        public string Type { get; set; }

        public int WorkflowId { get; set; }

        public int NextWorkflowStateId { get; set; }

        public string Manager { get; set; }

        public IList<EntityToPay> Entities { get; set; }
    }

    public class EntityToPay
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public int WorkflowId { get; set; }

        public int NextWorkflowStateId { get; set; }
    }
}
