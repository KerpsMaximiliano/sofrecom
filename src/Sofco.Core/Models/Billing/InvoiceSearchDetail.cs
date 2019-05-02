using System;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
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

        public int FileId { get; set; }
    }
}
