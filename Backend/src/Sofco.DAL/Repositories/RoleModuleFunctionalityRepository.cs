using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

        public IList<RoleModuleFunctionality> GetModulesByRoles(IEnumerable<int> roleIds)
        {
            return _context.RoleModuleFunctionality
                    .Include(x => x.Functionality)
                    .Include(x => x.Module)
                        .ThenInclude(x => x.Menu)
                    .Where(x => roleIds.Contains(x.RoleId) && x.Module != null)
                    .Distinct()
                    .ToList();
        }
    }
}
