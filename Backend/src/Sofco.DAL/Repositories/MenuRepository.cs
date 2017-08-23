using Sofco.Core.DAL;
using System.Collections.Generic;
using Sofco.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories
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
