using System;
using System.ComponentModel.DataAnnotations;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class HitoDetailModel
    {
        public HitoDetailModel()
        {
        }

        public HitoDetailModel(HitoDetail domain)
        {
            Id = domain.Id;
            Description = domain.Description;
            Quantity = domain.Quantity;
            UnitPrice = domain.UnitPrice;
            Total = domain.Total;
            HitoId = domain.HitoId;
        }

        public int Id { get; set; }

        public int HitoId { get; set; }

        [Required(ErrorMessage = "billing/solfac.hitoDescriptionRequired")]
        public string Description { get; set; }

        [Required(ErrorMessage = "billing/solfac.hitoQuantity")]
        public decimal? Quantity { get; set; }

        [Required(ErrorMessage = "billing/solfac.hitoUnitPriceRequired")]
        public decimal? UnitPrice { get; set; }

        public decimal Total { get; set; }

        public string ExternalHitoId { get; set; }

        public HitoDetail CreateDomain()
        {
            var detail = new HitoDetail
            {
                Id = Id,
                Description = Description,
                Quantity = Quantity.GetValueOrDefault(),
                UnitPrice = UnitPrice.GetValueOrDefault(),
                Total = Total,
                HitoId = HitoId,
                ExternalHitoId = ExternalHitoId,
                Modified = Id > 0 ? DateTime.UtcNow : (DateTime?)null
            };

            return detail;
        }
    }
}
