using System;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class InvoiceRowDetailModel
    {
        public InvoiceRowDetailModel(Invoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            CreatedDate = invoice.CreatedDate;
            InvoiceStatus = invoice.InvoiceStatus.ToString();

            if (invoice.PDfFileData != null)
            {
                ExcelFileName = invoice.PDfFileData.FileName;
                FileId = invoice.PdfFileId.GetValueOrDefault();
            }
            else if (invoice.ExcelFileData != null)
            {
                ExcelFileName = invoice.ExcelFileData.FileName;
                FileId = invoice.ExcelFileId.GetValueOrDefault();
            }
        }

        public int FileId { get; set; }

        public string ExcelFileName { get; set; }

        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceStatus { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Selected { get; set; }
    }
}
