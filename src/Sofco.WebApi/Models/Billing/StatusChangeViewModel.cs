using Sofco.Model.Enums;
using Sofco.Model.DTO;
using System;

namespace Sofco.WebApi.Models.Billing
{
    public class StatusChangeViewModel
    {
        public int UserId { get; set; }
        public string Comment { get; set; }
    }

    public class InvoiceStatusChangeViewModel : StatusChangeViewModel
    {
        public InvoiceStatus Status { get; set; }
        public string InvoiceNumber { get; set; }
    }

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

    public class UpdateSolfacCash
    {
        public int UserId { get; set; }
        public DateTime? CashedDate { get; set; }

        public SolfacStatusParams CreateStatusParams()
        {
            var parameters = new SolfacStatusParams();

            parameters.UserId = UserId;
            parameters.CashedDate = CashedDate;

            return parameters;
        }
    }
}
