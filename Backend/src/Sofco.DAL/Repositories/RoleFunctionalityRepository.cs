using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;

namespace Sofco.DAL.Repositories
{
    public class RoleFunctionalityRepository : BaseRepository<RoleFunctionality>, IRoleFunctionalityRepository
    {
        public RoleFunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int roleId, int functionalityId)
        {
            return _context.RoleFunctionality.Any(x => x.RoleId == roleId && x.FunctionalityId == functionalityId);
        }
    }
}
