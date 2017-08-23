using Sofco.Model.Models;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.DAL
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        bool ExistById(int id);
        IList<Role> GetRolesByGroup(IEnumerable<int> groupIds);
        Role GetDetail(int id);
        IList<Role> GetAllActivesReadOnly();
    }
}
