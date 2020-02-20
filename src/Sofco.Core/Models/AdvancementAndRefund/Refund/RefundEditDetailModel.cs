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
            Order = detail.Order;

            if (detail.CostType != null)
            {
                CostTypeId = detail.CostTypeId;
                CostTypeDesc = detail.CostType.Text;
            }
        }

        public int Order { get; set; }

        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal Ammount { get; set; }

        public string Description { get; set; }

        public int? CostTypeId { get; set; }

        public string CostTypeDesc { get; set; }
    }
}
