using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface ISolfacRepository : IBaseRepository<Solfac>
    {
        IList<Solfac> GetAllWithDocuments();
    }
}
