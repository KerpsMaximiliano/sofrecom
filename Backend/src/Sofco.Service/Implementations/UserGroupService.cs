using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using System.Collections.Generic;

namespace Sofco.Service.Implementations
{
    public class UserGroupService : BaseService<UserGroup>, IUserGroupService
    {
        IUserGroupRepository _repository;
        IRoleRepository _roleRepository;

        public UserGroupService(IUserGroupRepository repository, IBaseRepository<UserGroup> repoBase, IRoleRepository roleRepository) : base(repoBase)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        public Response<UserGroup> AddRole(int roleId, int userGroupId)
        {
            var response = new Response<UserGroup>();

            var role = _roleRepository.GetSingle(x => x.Id == roleId);

            if (role == null) {
                response.Messages.Add(new Message("Role no encontrado", MessageType.Error));
                return response;
            }

            var userGroup = GetById(userGroupId);

            if (userGroup == null)
            {
                response.Messages.Add(new Message("Grupo no encontrado", MessageType.Error));
                return response;
            }

            userGroup.Role = role;
            _repository.Update(userGroup);
            _repository.Save(currentUser);

            response.Messages.Add(new Message("Rol asignado al grupo correctamente", MessageType.Success));
            return response;
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

        public IList<UserGroup> GetAllReadOnlyWithEntitiesRelated()
        {
            return _repository.GetAllReadOnlyWithEntitiesRelated();
        }

        public UserGroup GetById(int id)
        {
            return _repository.GetSingle(x => x.Id == id);
        }

        public UserGroup GetByIdWithRole(int id)
        {
            return _repository.GetSingleWithRole(x => x.Id == id);
        }
    }
}
