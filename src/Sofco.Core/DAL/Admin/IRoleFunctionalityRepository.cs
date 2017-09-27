using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IRoleFunctionalityRepository : IBaseRepository<RoleFunctionality>
    {
        bool ExistById(int roleId, int functionalityId);
        IList<Functionality> GetFunctionalitiesByRoles(IEnumerable<int> roleIds);
    }
}
