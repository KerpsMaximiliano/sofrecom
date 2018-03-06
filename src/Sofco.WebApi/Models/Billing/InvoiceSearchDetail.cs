using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class InvoiceSearchDetail
    {
        public InvoiceSearchDetail(Invoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            AccountName = invoice.AccountName;
            Service = invoice.Service;
            ExcelFileName = string.IsNullOrWhiteSpace(invoice.PdfFileName) ? invoice.ExcelFileName : invoice.PdfFileName;
            Project = invoice.Project;
            ProjectId = invoice.ProjectId;
            CreatedDate = invoice.CreatedDate;
            StatusName = invoice.InvoiceStatus.ToString();

            if (invoice.User != null)
                User = invoice.User.Name;
        }

        public string ExcelFileName { get; set; }

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
