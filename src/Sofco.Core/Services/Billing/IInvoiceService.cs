using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Config;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IInvoiceService
    {
        IList<Invoice> GetByProject(string projectId);
        Response<Invoice> GetById(int id);
        Response<Invoice> Add(Invoice invoice, string identityName);
        //Response<Invoice> SaveExcel(Invoice responseData, string fileFileName);
        //Response<Invoice> GetExcel(int invoiceId);
        //Response<Invoice> SavePdf(Invoice responseData, string fileFileName);
        //Response<Invoice> GetPdf(int invoiceId);
        IList<Invoice> GetOptions(string projectId);
        Response Delete(int id);
        ICollection<Invoice> Search(InvoiceParams parameters);
        Response ChangeStatus(int invoiceId, InvoiceStatus status, EmailConfig emailConfig, InvoiceStatusParams parameters);
        Response<Invoice> Clone(int id);
        ICollection<InvoiceHistory> GetHistories(int id);
        Response RequestAnnulment(IList<int> invoices);
        Task<Response<File>> AttachFile(int invoiceId, Response<File> response, IFormFile file, string userName);
    }
}
