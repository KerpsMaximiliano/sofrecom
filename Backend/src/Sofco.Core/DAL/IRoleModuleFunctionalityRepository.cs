using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL
{
    public interface IRoleModuleFunctionalityRepository : IBaseRepository<RoleModuleFunctionality>
    {
        bool ExistById(int roleId, int moduleId, int functionalityId);
    }
}
