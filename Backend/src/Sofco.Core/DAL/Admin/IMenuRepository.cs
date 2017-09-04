using System.Collections.Generic;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IMenuRepository
    {
        IList<RoleFunctionality> GetFunctionalitiesByRoles(IEnumerable<int> roleIds);
    }
}
