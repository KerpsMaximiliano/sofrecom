using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace Sofco.DAL.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Roles.Any(x => x.Id == id);
        }

        public IList<Role> GetAllFullReadOnly()
        {
            return _context.Set<Role>()
                .Include(x => x.Groups)
                .Include(x => x.RoleModuleFunctionality)
                    .ThenInclude(x => x.Module)
                .Include(x => x.RoleModuleFunctionality)
                    .ThenInclude(x => x.Functionality)
                .ToList()
                .AsReadOnly();
        }

        public Role GetDetail(int id)
        {
            return _context.Set<Role>()
                    .Include(x => x.Groups)
                    .Include(x => x.RoleModuleFunctionality)
                        .ThenInclude(x => x.Module)
                    .Include(x => x.RoleModuleFunctionality)
                        .ThenInclude(x => x.Functionality)
                   .SingleOrDefault(x => x.Id == id);
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return _context.Groups
                    .Include(x => x.Role)
                    .Where(x => groupIds.Contains(x.Id) && x.Role != null)
                    .Select(x => x.Role)
                    .ToList();
        }
    }
}
