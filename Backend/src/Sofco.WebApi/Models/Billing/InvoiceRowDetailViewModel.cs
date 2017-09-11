using System;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class InvoiceRowDetailViewModel
    {
        public InvoiceRowDetailViewModel(Invoice invoice)
        {
            Id = invoice.Id;
            AccountName = invoice.AccountName;
            Cuit = invoice.Cuit;
            Service = invoice.Service;
            City = invoice.City;
            CreatedDate = invoice.CreatedDate;
            InvoiceStatus = invoice.InvoiceStatus.ToString();
        }

        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Cuit { get; set; }
        public string Service { get; set; }
        public string City { get; set; }
        public string InvoiceStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
