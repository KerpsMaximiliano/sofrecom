using System;
using Sofco.Model.DTO;

namespace Sofco.WebApi.Models.Billing
{
    public class UpdateSolfacBill
    {
        public string InvoiceCode { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public int UserId { get; set; }

        public SolfacStatusParams CreateStatusParams()
        {
            var parameters = new SolfacStatusParams();

            parameters.UserId = UserId;
            parameters.InvoiceCode = InvoiceCode;
            parameters.InvoiceDate = InvoiceDate;

            return parameters;
        }
    }
}
