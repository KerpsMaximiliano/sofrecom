using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.DAL.Billing
{
    public interface ISectorRepository : IBaseRepository<Sector>
    {
        new List<Sector> GetAll();
        IList<Sector> Get(IList<int> sectorIds);
    }
}