using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL
{
    public interface IRoleFunctionalityRepository : IBaseRepository<RoleFunctionality>
    {
        bool ExistById(int roleId, int functionalityId);
    }
}
