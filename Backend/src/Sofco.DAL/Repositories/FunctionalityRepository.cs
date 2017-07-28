using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL;
using Sofco.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace Sofco.DAL.Repositories
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

        public IList<Functionality> GetAllFullReadOnly()
        {
            return _context.Set<Functionality>()
                .Include(x => x.RoleFunctionality)
                    .ThenInclude(s => s.Role)
                .ToList();
        }

        public IList<Functionality> GetFuntionalitiesByRoles(IEnumerable<int> roleIds)
        {
            return _context.RoleFunctionality
                    .Include(x => x.Functionality)
                    .Where(x => roleIds.Contains(x.RoleId) && x.Functionality != null)
                    .Distinct()
                    .Select(x => x.Functionality)
                    .ToList();
        }

        public Functionality GetSingleWithRoles(Expression<Func<Functionality, bool>> predicate)
        {
            return _context.Set<Functionality>()
                .Include(x => x.RoleFunctionality)
                    .ThenInclude(s => s.Role)
                .SingleOrDefault(predicate);
        }
    }
}
