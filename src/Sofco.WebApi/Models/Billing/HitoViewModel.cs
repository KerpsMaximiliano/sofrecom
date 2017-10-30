using System;
using System.ComponentModel.DataAnnotations;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class HitoViewModel
    {
        public HitoViewModel()
        {
        }

        public HitoViewModel(Hito hito)
        {
            Id = hito.Id;
            Description = DescriptionOld = hito.Description;
            Quantity = hito.Quantity;
            UnitPrice = UnitPriceOld = hito.UnitPrice;
            Total = hito.Total;
            ExternalProjectId = hito.ExternalProjectId;
            ExternalHitoId = hito.ExternalHitoId;
            Currency = hito.Currency;
            Month = hito.Month;
        }

        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public short Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal Total { get; set; }

        public string ExternalProjectId { get; set; }

        public string ExternalHitoId { get; set; }

        public string Currency { get; set; }

        public short Month { get; set; }

        public string DescriptionOld { get; set; }

        public decimal UnitPriceOld { get; set; }

        public Hito CreateDomain()
        {
            var hito = new Hito();

            hito.Id = Id;
            hito.Description = Description;
            hito.Quantity = Quantity;
            hito.UnitPrice = UnitPrice;
            hito.Total = Total;
            hito.ExternalProjectId = ExternalProjectId;
            hito.ExternalHitoId = ExternalHitoId;
            hito.Currency = Currency;
            hito.Month = Month;
            hito.DescriptionOld = DescriptionOld;
            hito.UnitPriceOld = UnitPriceOld;

            return hito;
        }
    }

    public class HitoCrm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Money { get; set; }
        public decimal Ammount { get; set; }
        public DateTime StartDate { get; set; }
        public int Month { get; set; }
        public bool Billed { get; set; }
        public decimal AmmountBilled { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
    }
}
