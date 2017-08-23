using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Repositories
{
    public class RoleModuleRepository : BaseRepository<RoleModule>, IRoleModuleRepository
    {
        public RoleModuleRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int roleId, int moduleId)
        {
            return _context.RoleModule.Any(x => x.RoleId == roleId && x.ModuleId == moduleId);
        }

        public IList<Module> GetModulesByRoles(IEnumerable<int> roleIds)
        {
            return _context.RoleModule
                    .Include(x => x.Module)
                    .Where(x => roleIds.Contains(x.RoleId) && x.Module != null)
                    .Select(x => x.Module)
                    .Distinct()
                    .ToList();
        }
    }
}
