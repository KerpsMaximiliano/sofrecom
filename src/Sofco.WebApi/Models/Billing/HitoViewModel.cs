using System;
using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class HitoViewModel
    {
        public HitoViewModel(Hito hito)
        {
            Id = hito.Id;
            Description = hito.Description;
            Total = hito.Total;
            ExternalProjectId = hito.ExternalProjectId;
            ExternalHitoId = hito.ExternalHitoId;
            Currency = hito.Currency;
            Month = hito.Month;
            SolfacId = hito.SolfacId;
            CurrencyId = hito.CurrencyId;
            OpportunityId = hito.OpportunityId;
            ManagerId = hito.ManagerId;
        }

        public int Id { get; set; }

        public int SolfacId { get; set; }

        public string Description { get; set; }

        public decimal Total { get; set; }

        public string ExternalProjectId { get; set; }

        public string ExternalHitoId { get; set; }

        public string Currency { get; set; }

        public short Month { get; set; }

        public string CurrencyId { get; set; }

        public string OpportunityId { get; set; }

        public string ManagerId { get; set; }

        public Hito CreateDomain()
        {
            var hito = new Hito
            {
                Id = Id,
                Description = Description,
                Total = Total,
                ExternalProjectId = ExternalProjectId,
                ExternalHitoId = ExternalHitoId,
                Currency = Currency,
                Month = Month,
                SolfacId = SolfacId,
                CurrencyId = CurrencyId,
                OpportunityId = OpportunityId,
                ManagerId = ManagerId,
                Modified = Id > 0 ? DateTime.UtcNow : (DateTime?)null,
                Details = new List<HitoDetail>()
            };

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
