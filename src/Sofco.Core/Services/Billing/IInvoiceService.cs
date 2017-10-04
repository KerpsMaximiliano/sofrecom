using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IInvoiceService
    {
        IList<Invoice> GetByProject(string projectId);
        Response<Invoice> GetById(int id);
        Response<Invoice> Add(Invoice invoice, string identityName);
        Response<Invoice> SaveExcel(Invoice responseData);
        Response<Invoice> GetExcel(int invoiceId);
        Response<Invoice> SavePdf(Invoice responseData);
        Response<Invoice> GetPdf(int invoiceId);
        IList<Invoice> GetOptions(string projectId);
        Response Delete(int id);
        ICollection<Invoice> Search(InvoiceParams parameters, string userMail);
        Response ChangeStatus(int invoiceId, InvoiceStatus status, EmailConfig emailConfig, InvoiceStatusParams parameters);
        Response<Invoice> Clone(int id);
        ICollection<InvoiceHistory> GetHistories(int id);
    }
}
