using Sofco.Model.Models;

namespace Sofco.Core.Interfaces.DAL
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        bool Exist(int id);
    }
}
