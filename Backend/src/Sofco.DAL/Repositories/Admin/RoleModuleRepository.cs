using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Admin
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
