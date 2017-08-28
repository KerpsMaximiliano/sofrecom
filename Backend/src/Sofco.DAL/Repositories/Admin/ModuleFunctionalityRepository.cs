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
