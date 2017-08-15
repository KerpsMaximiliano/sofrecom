using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IRoleModuleFunctionalityRepository : IBaseRepository<RoleModuleFunctionality>
    {
        bool ExistById(int roleId, int moduleId, int functionalityId);
        IList<RoleModuleFunctionality> GetModulesByRoles(IEnumerable<int> roleIds);
    }
}
