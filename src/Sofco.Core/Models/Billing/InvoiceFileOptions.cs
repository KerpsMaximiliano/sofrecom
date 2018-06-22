using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class InvoiceFileOptions
    {
        public InvoiceFileOptions(Invoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            InvoiceStatus = invoice.InvoiceStatus;
            SolfacId = invoice.SolfacId;

            if (invoice.ExcelFileData != null)
            {
                ExcelFileId = invoice.ExcelFileId;
                ExcelFileName = invoice.ExcelFileData.FileName;
            }

            if (invoice.PDfFileData != null)
            {
                PdfFileId = invoice.PdfFileId;
                PdfFileName = invoice.PDfFileData.FileName;
            }
        }

        public int? ExcelFileId { get; set; }

        public string ExcelFileName { get; set; }

        public int? PdfFileId { get; set; }

        public string PdfFileName { get; set; }

        public int? SolfacId { get; set; }

        public string InvoiceNumber { get; set; }
        public InvoiceStatus InvoiceStatus { get; }

        public int Id { get; set; }
    }
}
