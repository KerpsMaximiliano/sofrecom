using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;

namespace Sofco.DAL.Repositories
{
    public class RoleModuleFunctionalityRepository : BaseRepository<RoleModuleFunctionality>, IRoleModuleFunctionalityRepository
    {
        public RoleModuleFunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int roleId, int moduleId, int functionalityId)
        {
            return _context.RoleModuleFunctionality.Any(x => x.RoleId == roleId && x.FunctionalityId == functionalityId && x.ModuleId == moduleId);
        }
    }
}
