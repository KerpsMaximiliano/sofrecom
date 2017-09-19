using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacSearchDetail
    {
        public SolfacSearchDetail(Solfac domain)
        {
            Id = domain.Id;
            Project = domain.Project;
            BusinessName = domain.BusinessName;
            DocumentTypeName = domain.DocumentType.Text;
            StartDate = domain.StartDate;
            Amount = domain.Amount;
            Iva21 = domain.Iva21;
            TotalAmount = domain.TotalAmount;
            StatusName = domain.Status.ToString();
            CurrencyId = domain.CurrencyId;
        }

        public int Id { get; set; }
        public string Project { get; set; }
        public string BusinessName { get; set; }
        public string DocumentTypeName { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Iva21 { get; set; }
        public decimal TotalAmount { get; set; }
        public string StatusName { get; set; }
        public int CurrencyId { get; set; }
    }
}
