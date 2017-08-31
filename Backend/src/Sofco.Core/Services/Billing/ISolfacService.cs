using System.Collections.Generic;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacService
    {
        Response<Solfac> Add(Solfac solfac);
        IList<Solfac> GetAll();
        IList<Hito> GetHitosByProject(string projectId);
        IList<Solfac> GetByProject(string projectId);
        Response<Solfac> GetById(int id);
    }
}
