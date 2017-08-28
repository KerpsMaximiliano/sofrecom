using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class MenuRepository : IMenuRepository
    {
        protected readonly SofcoContext _context;

        public MenuRepository(SofcoContext context)
        {
            _context = context;
        }

        public IList<RoleModule> GetMenuByRoles(IEnumerable<int> roleIds)
        {
            return _context.RoleModule
                .Include(x => x.Module) 
                    .ThenInclude(x => x.Menu)
                .Include(x => x.Module)
                    .ThenInclude(x => x.ModuleFunctionality)
                        .ThenInclude(x => x.Functionality)
                .Where(x => roleIds.Contains(x.RoleId) && x.Module != null)
                .Distinct()
                .ToList();
        }
    }
}
