using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        bool ExistById(int id);

        IList<Role> GetRolesByGroup(IEnumerable<int> groupIds);

        Role GetDetail(int id);

        IList<Role> GetAllActivesReadOnly();

        bool ExistByDescription(string roleDescription, int roleId);

        Role GetByCode(string code);
    }
}
