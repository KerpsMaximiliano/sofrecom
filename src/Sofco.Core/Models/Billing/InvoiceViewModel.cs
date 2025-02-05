﻿using System;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class InvoiceViewModel
    {
        public InvoiceViewModel()
        {
        }

        public InvoiceViewModel(Invoice invoice)
        {
            Id = invoice.Id;
            AccountName = invoice.AccountName;
            Address = invoice.Address;
            Zipcode = invoice.Zipcode;
            City = invoice.City;
            Province = invoice.Province;
            Country = invoice.Country;
            Cuit = invoice.Cuit;
            Service = invoice.Service;
            Project = invoice.Project;
            ProjectId = invoice.ProjectId;
            Analytic = invoice.Analytic;
       
            CreatedDate = invoice.CreatedDate;
            InvoiceStatus = invoice.InvoiceStatus.ToString();
            InvoiceNumber = invoice.InvoiceNumber;
            ServiceId = invoice.ServiceId;
            CustomerId = invoice.AccountId;

            if (invoice.ExcelFileData != null)
            {
                ExcelFileId = invoice.ExcelFileId;
                ExcelFileName = invoice.ExcelFileData.FileName;
                ExcelFileCreatedDate = invoice.ExcelFileData.CreationDate.ToString("d");
            }

            if (invoice.PDfFileData != null)
            {
                PdfFileId = invoice.PdfFileId;
                PdfFileName = invoice.PDfFileData.FileName;
                PdfFileCreatedDate = invoice.PDfFileData.CreationDate.ToString("d");
            }

            if (invoice.Solfac != null)
            {
                SolfacId = invoice.Solfac.Id;
            }
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

        public string ServiceId { get; set; }

        public string CustomerId { get; set; }

        public string Analytic { get; set; }

        public string ExcelFileName { get; set; }

        public string PdfFileName { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceStatus { get; set; }

        public string ExcelFileCreatedDate { get; set; }

        public string PdfFileCreatedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? SolfacId { get; set; }

        public int? ExcelFileId { get; set; }

        public int? PdfFileId { get; set; }

        public Invoice CreateDomain()
        {
            var domain = new Invoice();

            domain.AccountName = AccountName;
            domain.Address = Address;
            domain.Zipcode = Zipcode;
            domain.City = City;
            domain.Province = Province;
            domain.Country = Country;
            domain.Cuit = Cuit;
            domain.Service = Service;
            domain.Project = Project;
            domain.ProjectId = ProjectId;
            domain.Analytic = Analytic;
            domain.AccountId = CustomerId;
            domain.ServiceId = ServiceId;

            return domain;
        }
    }
}