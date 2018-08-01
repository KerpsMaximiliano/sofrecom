using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class RoleFunctionalityRepository : BaseRepository<RoleFunctionality>, IRoleFunctionalityRepository
    {
        public RoleFunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int roleId, int functionalityId)
        {
            return context.RoleFunctionality.Any(x => x.RoleId == roleId && x.FunctionalityId == functionalityId);
        }

        public IList<Functionality> GetFunctionalitiesByRoles(IEnumerable<int> roleIds)
        {
            return context.RoleFunctionality
                    .Include(x => x.Functionality)
                    .Where(x => roleIds.Contains(x.RoleId) && x.Role != null)
                    .Select(x => x.Functionality)
                    .Distinct()
                    .ToList();
        }
    }
}
