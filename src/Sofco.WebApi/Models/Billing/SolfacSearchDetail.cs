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

            if (domain.DocumentType != null)
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

    public class InvoiceSearchDetail
    {
        public InvoiceSearchDetail(Invoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            AccountName = invoice.AccountName;
            Service = invoice.Service;
            Project = invoice.Project;
            ProjectId = invoice.ProjectId;
            CreatedDate = invoice.CreatedDate;
            StatusName = invoice.InvoiceStatus.ToString();

            if (invoice.User != null)
                User = invoice.User.Name;
        }

        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string AccountName { get; set; }
        public string Service { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }
        public string User { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusName { get; set; }
    }
}
