using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Core.DAL
{
    public interface IModuleFunctionalityRepository : IBaseRepository<ModuleFunctionality>
    {
        bool ExistById(int moduleId, int functionalityId);
        IList<Functionality> GetFuntionalitiesByModule(IEnumerable<int> moduleIds);
    }
}
