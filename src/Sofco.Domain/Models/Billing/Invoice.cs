using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sofco.Domain.Models.Common;

namespace Sofco.Domain.Models.Billing
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

        public InvoiceStatus InvoiceStatus { get; set; }

        public string InvoiceNumber { get; set; }
         
        public int UserId { get; set; }
        public User User { get; set; }

        public int? SolfacId { get; set; }
        public Solfac Solfac { get; set; }

        public string AccountId { get; set; }
        public string ServiceId { get; set; }

        public int? ExcelFileId { get; set; }
        public File ExcelFileData { get; set; }

        public int? PdfFileId { get; set; }
        public File PDfFileData { get; set; }

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
            invoice.AccountId = this.AccountId;
            invoice.Province = this.Province;
            invoice.Zipcode = this.Zipcode;
            invoice.UserId = this.UserId;
            invoice.InvoiceNumber = "0000-00000000";
            invoice.CreatedDate = DateTime.Now;
            invoice.InvoiceStatus = InvoiceStatus.SendPending;

            return invoice;
        }
    }
}
