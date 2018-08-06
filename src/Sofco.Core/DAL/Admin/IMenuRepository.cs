using System.Collections.Generic;
using Sofco.Domain.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IMenuRepository
    {
        IList<RoleFunctionality> GetFunctionalitiesByRoles(IEnumerable<int> roleIds);
    }
}
