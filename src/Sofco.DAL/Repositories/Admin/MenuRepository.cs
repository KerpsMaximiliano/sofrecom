using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Models.Admin;
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

        public IList<RoleFunctionality> GetFunctionalitiesByRoles(IEnumerable<int> roleIds)
        {
            return _context.RoleFunctionality
                .Where(x => roleIds.Contains(x.RoleId))
                .Include(x => x.Functionality)
                    .ThenInclude(x => x.Module)
                .ToList();
        }
    }
}
