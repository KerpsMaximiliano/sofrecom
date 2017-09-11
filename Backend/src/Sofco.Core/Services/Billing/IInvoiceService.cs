using System.Collections.Generic;
using Sofco.Core.Config;
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
        Response<Invoice> SendToDaf(int invoiceId, EmailConfig emailConfig);
        Response<Invoice> GetExcel(int invoiceId);
        Response<Invoice> SavePdf(Invoice responseData);
        Response<Invoice> GetPdf(int invoiceId);
        Response<Invoice> Reject(int invoiceId);
        Response<Invoice> Approve(int invoiceId, string invoiceNumber);
        IList<Invoice> GetOptions(string projectId);
        Response Delete(int id);
        Response<Invoice> Annulment(int invoiceId);
    }
}
