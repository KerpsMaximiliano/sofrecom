using System;
using Sofco.Model.DTO;
using Sofco.Model.Enums;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacStatusChangeViewModel : StatusChangeViewModel
    {
        public SolfacStatus Status { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? CashedDate { get; set; }

        public SolfacStatusParams CreateStatusParams()
        {
            var parameters = new SolfacStatusParams();

            parameters.UserId = UserId;
            parameters.Comment = Comment;
            parameters.Status = Status;
            parameters.InvoiceCode = InvoiceCode;
            parameters.InvoiceDate = InvoiceDate;
            parameters.CashedDate = CashedDate;

            return parameters;
        }
    }
}
