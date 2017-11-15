using System.ComponentModel.DataAnnotations;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class HitoDetailViewModel
    {
        public HitoDetailViewModel()
        {
             
        }

        public HitoDetailViewModel(HitoDetail domain)
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
        public short? Quantity { get; set; }

        [Required(ErrorMessage = "billing/solfac.hitoUnitPriceRequired")]
        public decimal? UnitPrice { get; set; }

        public decimal Total { get; set; }

        public string ExternalHitoId { get; set; }

        public HitoDetail CreateDomain()
        {
            var detail = new HitoDetail();

            detail.Id = Id;
            detail.Description = Description;
            detail.Quantity = Quantity.GetValueOrDefault();
            detail.UnitPrice = UnitPrice.GetValueOrDefault();
            detail.Total = Total;
            detail.HitoId = HitoId;
            detail.ExternalHitoId = ExternalHitoId;

            return detail;
        }
    }
}
