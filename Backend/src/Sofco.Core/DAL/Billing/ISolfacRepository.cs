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
    }
}
