using Sofco.Model.Models;

namespace Sofco.Core.Interfaces.Services
{
    public interface IRoleService : IBaseService<Role>
    {
        void DeleteById(int id);

        Role GetById(int id);
    }
}
