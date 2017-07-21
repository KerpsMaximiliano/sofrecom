using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;

namespace Sofco.Service.Implementations
{
    public class UserGroupService : BaseService<UserGroup>, IUserGroupService
    {
        IUserGroupRepository _repository;

        public UserGroupService(IUserGroupRepository repository, IBaseRepository<UserGroup> repoBase) : base(repoBase)
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

        public UserGroup GetById(int id)
        {
            return _repository.GetSingle(x => x.Id == id);
        }
    }
}
