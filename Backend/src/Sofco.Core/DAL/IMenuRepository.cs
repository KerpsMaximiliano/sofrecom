using Sofco.Model.Models;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IMenuRepository
    {
        IList<RoleModule> GetMenuByRoles(IEnumerable<int> roleIds);
    }
}
