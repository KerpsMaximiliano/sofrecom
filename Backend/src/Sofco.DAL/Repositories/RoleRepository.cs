using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;

namespace Sofco.DAL.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(SofcoContext context) : base(context)
        {
        }
    }
}
