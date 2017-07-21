using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq;

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
    }
}
