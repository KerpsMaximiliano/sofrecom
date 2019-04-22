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

        public decimal AmmountPesos { get; set; }

        public string Type { get; set; }

        public int WorkflowId { get; set; }

        public int NextWorkflowStateId { get; set; }

        public decimal? CurrencyExchange { get; set; }

        public string Manager { get; set; }
    }
}
