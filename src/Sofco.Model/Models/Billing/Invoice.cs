using System;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sofco.Model.Models.Billing
{
    public class Invoice : BaseEntity
    {
        public Invoice()
        {
            Histories = new Collection<InvoiceHistory>();
            CreatedDate = DateTime.Now;
        }

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

        public DateTime CreatedDate { get; set; }
        public byte[] ExcelFile { get; set; }
        public byte[] PdfFile { get; set; }
        public string ExcelFileName { get; set; }
        public string PdfFileName { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }
        public DateTime ExcelFileCreatedDate { get; set; }
        public DateTime PdfFileCreatedDate { get; set; }
        public string InvoiceNumber { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public Solfac Solfac { get; set; }

        public string CustomerId { get; set; }
        public string ServiceId { get; set; }

        public ICollection<InvoiceHistory> Histories { get; set; }

        public Invoice Clone()
        {
            var invoice = new Invoice();

            invoice.AccountName = this.AccountName;
            invoice.Address = this.Address;
            invoice.Analytic = this.Analytic;
            invoice.City = this.City;
            invoice.Country = this.Country;
            invoice.Cuit = this.Cuit;
            invoice.Project = this.Project;
            invoice.ProjectId = this.ProjectId;
            invoice.ProjectId = this.ProjectId;
            invoice.Service = this.Service;
            invoice.ServiceId = this.ServiceId;
            invoice.CustomerId = this.CustomerId;
            invoice.Province = this.Province;
            invoice.Zipcode = this.Zipcode;
            invoice.UserId = this.UserId;
            invoice.InvoiceNumber = "0000-00000000";
            invoice.CreatedDate = DateTime.Now;
            invoice.InvoiceStatus = InvoiceStatus.SendPending;

            invoice.ExcelFile = this.ExcelFile;
            invoice.ExcelFileName = this.ExcelFileName;
            invoice.ExcelFileCreatedDate = this.ExcelFileCreatedDate;

            return invoice;
        }
    }
}
