using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL
{
    public interface IRoleMenuRepository : IBaseRepository<RoleMenu>
    {
        bool ExistById(int roleId, int menuId);
        bool MenuExistById(int menuId);        
    }
}
