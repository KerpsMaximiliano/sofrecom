using System;
using System.Collections.Generic;

namespace Sofco.Model.Models.Billing
{
    public class Invoice
    {
        public Invoice()
        {
            Details = new List<InvoiceDetail>();
        }

        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Cuit { get; set; }
        public string Service { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }
        public string Analytic { get; set; }

        public IList<InvoiceDetail> Details { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class InvoiceDetail
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
