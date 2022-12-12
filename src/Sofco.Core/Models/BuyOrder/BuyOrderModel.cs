using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.BuyOrder
{
    public class BuyOrderModel
    {
        public int Id { get; set; }

        public int? UserApplicantId { get; set; }

        public string Number { get; set; }

        public int ProviderId { get; set; }

        public decimal? TotalAmount { get; set; }

        public IList<BuyOrderDetailModel> Items { get; set; }
        public Domain.Models.RequestNote.BuyOrder CreateDomain()
        {
            var domain = new Domain.Models.RequestNote.BuyOrder();

            domain.BuyOrderNumber = Number;
            domain.ProviderId = ProviderId;
            domain.TotalAmmount = TotalAmount;
            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.InWorkflowProcess = true;
            domain.CreationDate = DateTime.UtcNow;
            foreach (var detail in Items)
            {
                domain.ProductsServices.Add(detail.CreateDomain());
            }
            return domain;
        }
    }
    public class BuyOrderDetailModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int Quantity { get; set; }

        public int RequestNoteProductServiceId { get; set; }

        public BuyOrderProductService CreateDomain()
        {
            var domain = new BuyOrderProductService();
            domain.Price = Amount;
            domain.Quantity = Quantity;
            domain.RequestNoteProductServiceId = RequestNoteProductServiceId;

            return domain;
        }
    }
}
