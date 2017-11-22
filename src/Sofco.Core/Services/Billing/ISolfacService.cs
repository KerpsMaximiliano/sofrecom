using System.Collections.Generic;
using System.Threading.Tasks;
using Sofco.Core.Config;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacService
    {
        Response<Solfac> Add(Solfac solfac, IList<int> invoicesId);
        IList<Solfac> Search(SolfacParams parameter, string userMail);
        IList<Hito> GetHitosByProject(string projectId);
        IList<Solfac> GetByProject(string projectId);
        Response<Solfac> GetById(int id);
        Response ChangeStatus(Solfac solfac, SolfacStatusParams parameters, EmailConfig emailConfig);
        Response ChangeStatus(int solfacId, SolfacStatusParams parameters, EmailConfig emailConfig);
        Response Delete(int id);
        ICollection<SolfacHistory> GetHistories(int id);
        Response Update(Solfac domain, string modelComments);
        Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName);
        ICollection<SolfacAttachment> GetFiles(int solfacId);
        Response<SolfacAttachment> GetFileById(int fileId);
        Response DeleteFile(int id);
        Response UpdateBill(int id, SolfacStatusParams solfacStatusParams);
        Response UpdateCashedDate(int id, SolfacStatusParams parameters);
        Response DeleteInvoice(int id, int invoiceId);
        Response<ICollection<Invoice>> GetInvoices(int id);
        Response AddInvoices(int id, IList<int> invoices);
        Response<Solfac> Validate(Solfac solfac);
        Response DeleteDetail(int id);
        Task<Response> SplitHito(HitoSplittedParams hito);

        Response<Solfac> Post(Solfac solfac, IList<int> invoicesId);
    }
}
