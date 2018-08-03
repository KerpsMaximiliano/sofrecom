using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using System;
using Sofco.Core.Models.Admin;

namespace Sofco.Core.DAL.Billing
{
    public interface ISolfacRepository : IBaseRepository<Solfac>
    {
        IList<Solfac> GetAllWithDocuments();
        IList<Hito> GetHitosByProject(string projectId);
        IList<Solfac> GetByProject(string projectId);
        Solfac GetById(int id);
        IList<Solfac> SearchByParams(SolfacParams parameters);
        void UpdateStatus(Solfac solfacToModif);
        Solfac GetByIdWithUser(int id);
        ICollection<SolfacHistory> GetHistories(int solfacId);
        void AddHistory(SolfacHistory history);
        void SaveAttachment(SolfacAttachment attachment);
        ICollection<SolfacAttachment> GetFiles(int solfacId);
        SolfacAttachment GetFileById(int fileId);
        void DeleteFile(SolfacAttachment file);
        void UpdateStatusAndInvoice(Solfac solfacToModif);
        ICollection<string> GetHitosIdsBySolfacId(int solfacId);
        void UpdateStatusAndCashed(Solfac solfacToModif);
        IList<Solfac> SearchByParamsAndUser(SolfacParams parameter, UserLiteModel userMail);
        void UpdateInvoice(Solfac solfacToModif);
        void UpdateCash(Solfac solfacToModif);
        bool InvoiceCodeExist(string invoiceCode);
        IList<Hito> GetHitosByExternalIds(List<Guid> externalIds);
        HitoDetail GetDetail(int id);
        void DeleteDetail(HitoDetail detail);
        bool HasAttachments(int solfacId);
        bool HasInvoices(int solfacId);
        IList<Hito> GetHitosBySolfacId(int solfacId);

        decimal GetTotalAmountById(int solfacId);

        IEnumerable<Solfac> GetByProjectWithPurchaseOrder(string projectId);
    }
}
