using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IRoleModuleRepository : IBaseRepository<RoleModule>
    {
        bool ExistById(int roleId, int moduleId);
        IList<Module> GetModulesByRoles(IEnumerable<int> roleIds);
    }
}
