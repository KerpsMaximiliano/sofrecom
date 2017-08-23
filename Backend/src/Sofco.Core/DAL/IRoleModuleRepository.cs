using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IRoleModuleRepository : IBaseRepository<RoleModule>
    {
        bool ExistById(int roleId, int moduleId);
        IList<Module> GetModulesByRoles(IEnumerable<int> roleIds);
    }
}
