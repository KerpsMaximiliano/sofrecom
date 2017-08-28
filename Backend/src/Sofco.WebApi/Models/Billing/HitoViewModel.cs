using System.ComponentModel.DataAnnotations;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class HitoViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
        [Required]
        public short Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public Hito CreateDomain()
        {
            var hito = new Hito();

            hito.Description = Description;
            hito.Quantity = Quantity;
            hito.UnitPrice = UnitPrice;
            hito.Total = Total;

            return hito;
        }
    }
}
