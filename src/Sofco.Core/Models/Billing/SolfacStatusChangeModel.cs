using System;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Billing
{
    public class SolfacStatusChangeModel : StatusChangeModel
    {
        public SolfacStatus Status { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? CashedDate { get; set; }

        public decimal CurrencyExchange { get; set; }

        public SolfacStatusParams CreateStatusParams()
        {
            var parameters = new SolfacStatusParams();

            parameters.UserId = UserId;
            parameters.Comment = Comment;
            parameters.Status = Status;
            parameters.InvoiceCode = InvoiceCode;
            parameters.InvoiceDate = InvoiceDate;
            parameters.CashedDate = CashedDate;
            parameters.CurrencyExchange = CurrencyExchange;

            return parameters;
        }
    }
}
