using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.DAL.Billing
{
    public interface IAreaRepository : IBaseRepository<Area>
    {
        new List<Area> GetAll();
        Area GetWithResponsable(int id);
    }
}