using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IModuleFunctionalityRepository : IBaseRepository<ModuleFunctionality>
    {
        bool ExistById(int moduleId, int functionalityId);
        IList<Functionality> GetFuntionalitiesByModule(IEnumerable<int> moduleIds);
    }
}
