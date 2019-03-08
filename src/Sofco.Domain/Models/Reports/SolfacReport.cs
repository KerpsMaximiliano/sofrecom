using System;

namespace Sofco.Domain.Models.Reports
{
    public class SolfacReport
    {
        public int Id { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string BusinessName { get; set; }

        public string AccountName { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal Amount { get; set; }
    }
}
