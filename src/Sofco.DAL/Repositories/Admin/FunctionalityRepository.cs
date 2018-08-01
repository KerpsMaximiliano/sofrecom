using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class FunctionalityRepository : BaseRepository<Functionality>, IFunctionalityRepository
    {
        public FunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return context.Set<Functionality>().Any(x => x.Id == id);
        }

        public IList<Functionality> GetAllActivesReadOnly()
        {
            return context.Set<Functionality>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Functionality> GetFuntionalitiesByModule(IEnumerable<int> modules)
        {
            return context.Functionalities.Where(x => modules.Contains(x.ModuleId)).ToList();
        }

        public IList<RoleFunctionality> GetFuntionalitiesByRole(IEnumerable<int> roles)
        {
            return context.RoleFunctionality
                .Where(x => roles.Contains(x.RoleId))
                .Include(x => x.Functionality)
                .ThenInclude(x => x.Module)
                .Distinct()
                .ToList();
        }
    }
}
