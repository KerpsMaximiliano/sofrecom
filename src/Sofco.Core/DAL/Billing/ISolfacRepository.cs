using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;

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
        void UpdateStatusAndInvoiceCode(Solfac solfacToModif);
        ICollection<string> GetHitosIdsBySolfacId(int solfacId);
    }
}
