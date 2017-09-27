using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Admin
{
    public class FunctionalityRepository : BaseRepository<Functionality>, IFunctionalityRepository
    {
        public FunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Set<Functionality>().Any(x => x.Id == id);
        }

        public IList<Functionality> GetAllActivesReadOnly()
        {
            return _context.Set<Functionality>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Functionality> GetFuntionalitiesByModule(IEnumerable<int> modules)
        {
            return _context.Functionalities.Where(x => modules.Contains(x.ModuleId)).ToList();
        }

        public IList<RoleFunctionality> GetFuntionalitiesByRole(IEnumerable<int> roles)
        {
            return _context.RoleFunctionality
                .Where(x => roles.Contains(x.RoleId))
                .Include(x => x.Functionality)
                .ThenInclude(x => x.Module)
                .Distinct()
                .ToList();
        }
    }
}
