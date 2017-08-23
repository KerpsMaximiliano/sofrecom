using Sofco.Core.DAL;
using Sofco.Model.Relationships;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Repositories
{
    public class ModuleFunctionalityRepository : BaseRepository<ModuleFunctionality>, IModuleFunctionalityRepository
    {
        public ModuleFunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int moduleId, int functionalityId)
        {
            return _context.ModuleFunctionality.Any(x => x.ModuleId == moduleId && x.FunctionalityId == functionalityId);
        }

        public IList<Functionality> GetFuntionalitiesByModule(IEnumerable<int> moduleIds)
        {
            return _context.ModuleFunctionality
                    .Include(x => x.Functionality)
                    .Where(x => moduleIds.Contains(x.ModuleId) && x.Module != null)
                    .Select(x => x.Functionality)
                    .Distinct()
                    .ToList();
        }
    }
}
