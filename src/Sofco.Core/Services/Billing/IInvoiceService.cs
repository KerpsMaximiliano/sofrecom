using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Config;
using Sofco.Core.Models.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Core.Services.Billing
{
    public interface IInvoiceService
    {
        IList<Invoice> GetByProject(string projectId);
        Response<Invoice> GetById(int id);
        Response<Invoice> Add(Invoice invoice, string identityName);
        IList<Invoice> GetOptions(string projectId);
        Response Delete(int id);
        ICollection<Invoice> Search(InvoiceParams parameters);
        Response ChangeStatus(int invoiceId, InvoiceStatus status, EmailConfig emailConfig, InvoiceStatusParams parameters);
        Response<Invoice> Clone(int id);
        ICollection<InvoiceHistory> GetHistories(int id);
        Response RequestAnnulment(InvoiceAnnulmentModel model);
        Task<Response<File>> AttachFile(int invoiceId, Response<File> response, IFormFile file, string userName);
        Response<Stream> GetZip(IList<int> ids);
    }
}
