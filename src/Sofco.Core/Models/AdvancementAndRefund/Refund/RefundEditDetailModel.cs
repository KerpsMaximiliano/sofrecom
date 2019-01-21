using System;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundEditDetailModel
    {
        public RefundEditDetailModel(RefundDetail detail)
        {
            Id = detail.Id;
            CreationDate = detail.CreationDate;
            Ammount = detail.Ammount;
            Description = detail.Description;
            AnalyticId = detail.AnalyticId;
        }

        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal Ammount { get; set; }

        public string Description { get; set; }

        public int AnalyticId { get; set; }
    }
}
