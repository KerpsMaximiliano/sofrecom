using System;
using Sofco.Domain.DTO;

namespace Sofco.Core.Models.Billing
{
    public class UpdateSolfacBill
    {
        public string InvoiceCode { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? CurrencyExchange { get; set; }

        public int UserId { get; set; }

        public SolfacStatusParams CreateStatusParams()
        {
            var parameters = new SolfacStatusParams();

            parameters.UserId = UserId;
            parameters.InvoiceCode = InvoiceCode;
            parameters.InvoiceDate = InvoiceDate;
            parameters.CurrencyExchange = CurrencyExchange;

            return parameters;
        }
    }
}
