using System.Collections.Generic;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IMenuRepository
    {
        IList<RoleModule> GetMenuByRoles(IEnumerable<int> roleIds);
    }
}
