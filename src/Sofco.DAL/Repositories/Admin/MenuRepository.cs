using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class MenuRepository : IMenuRepository
    {
        protected readonly SofcoContext Context;

        public MenuRepository(SofcoContext context)
        {
            Context = context;
        }

        public IList<RoleFunctionality> GetFunctionalitiesByRoles(IEnumerable<int> roleIds)
        {
            return Context.RoleFunctionality
                .Where(x => roleIds.Contains(x.RoleId))
                .Include(x => x.Functionality)
                    .ThenInclude(x => x.Module)
                .Distinct()
                .ToList();
        }
    }
}
