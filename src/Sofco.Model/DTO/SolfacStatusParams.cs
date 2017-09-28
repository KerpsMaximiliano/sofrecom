using System;
using Sofco.Model.Enums;

namespace Sofco.Model.DTO
{
    public class SolfacStatusParams
    {
        public SolfacStatusParams(int userId, string comment, string invoiceCode, SolfacStatus status)
        {
            Status = status;
            Comment = comment;
            InvoiceCode = invoiceCode;
            UserId = userId;
        }

        public SolfacStatusParams(int userId, string comment, string invoiceCode, SolfacStatus status, DateTime? invoiceDate) : this(userId, comment, invoiceCode, status)
        {
            this.InvoiceDate = invoiceDate;
        }

        public SolfacStatus Status { get; set; }
        public string Comment { get; set; }
        public string InvoiceCode { get; set; }
        public int UserId { get; set; }
        public DateTime? InvoiceDate;
    }
}
