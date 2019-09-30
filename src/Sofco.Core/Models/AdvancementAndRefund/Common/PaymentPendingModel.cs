using System.Collections.Generic;

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

        public bool CanPayAll { get; set; }

        public decimal AmmountPesos
        {
            get
            {
                if (IsCurrencyPesos) return Ammount;

                if (CurrencyExchange.HasValue)
                {
                    return this.Ammount * this.CurrencyExchange.Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal? CurrencyExchange { get; set; }

        public bool IsCurrencyPesos { get; set; }
    }

    public class EntityToPay
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public int WorkflowId { get; set; }

        public int NextWorkflowStateId { get; set; }

        public decimal Ammount { get; set; }

        public string EntitiesRelatedDesc { get; set; }

        public IEnumerable<int> EntitiesRelatedIds { get; set; }

        public string EntityType { get; set; }

        public string CurrencyName { get; set; }
    }
}
