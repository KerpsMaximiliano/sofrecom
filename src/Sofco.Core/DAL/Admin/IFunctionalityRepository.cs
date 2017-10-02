using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IFunctionalityRepository : IBaseRepository<Functionality>
    {
        bool ExistById(int id);
        IList<Functionality> GetAllActivesReadOnly();
        IList<Functionality> GetFuntionalitiesByModule(IEnumerable<int> modules);
        IList<RoleFunctionality> GetFuntionalitiesByRole(IEnumerable<int> roles);
    }
}
