using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;

namespace Sofco.DAL.Repositories
{
    public class RoleMenuRepository : BaseRepository<RoleMenu>, IRoleMenuRepository
    {
        public RoleMenuRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int roleId, int functionalityId)
        {
            return _context.RoleFunctionality.Any(x => x.RoleId == roleId && x.FunctionalityId == functionalityId);
        }

        public bool MenuExistById(int menuId)
        {
            return _context.Menus.Any(x => x.Id == menuId);
        }
    }
}
