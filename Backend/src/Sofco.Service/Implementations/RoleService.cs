using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;

namespace Sofco.Service.Implementations
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        IRoleRepository _repository;

        public RoleService(IRoleRepository repository, IBaseRepository<Role> repoBase) : base(repoBase)
        {
            _repository = repository;
        }

        public void DeleteById(int id)
        {
            var entity = _repository.GetSingle(x => x.Id == id);

            if(entity != null)
            {
                _repository.Delete(entity);
                _repository.Save(currentUser);
            }
        }

        public Role GetById(int id)
        {
            return _repository.GetSingle(x => x.Id == id);
        }
    }
}
