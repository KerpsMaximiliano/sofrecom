using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Role> GetAllActivesReadOnly()
        {
            return _context.Set<Role>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public Role GetDetail(int id)
        {
            return _context.Set<Role>()
                    .Include(x => x.Groups)
                    .Include(x => x.RoleModule)
                        .ThenInclude(x => x.Module)
                            .ThenInclude(x => x.ModuleFunctionality)
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
