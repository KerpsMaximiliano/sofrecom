﻿using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class InvoiceRowDetailViewModel
    {
        public InvoiceRowDetailViewModel(Invoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            CreatedDate = invoice.CreatedDate;
            InvoiceStatus = invoice.InvoiceStatus.ToString();
            ExcelFileName = string.IsNullOrWhiteSpace(invoice.PdfFileName) ? invoice.ExcelFileName : invoice.PdfFileName;
        }

        public string ExcelFileName { get; set; }

        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceStatus { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
