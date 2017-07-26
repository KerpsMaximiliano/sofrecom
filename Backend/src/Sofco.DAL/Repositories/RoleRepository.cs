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

        public bool Exist(int id)
        {
            return _context.Roles.Any(x => x.Id == id);
        }

        public override IList<Role> GetAllReadOnly()
        {
            return _context.Set<Role>().Include(x => x.Groups).ToList();
        }
    }
}
