using System;
using System.Collections.Generic;
using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Core.DAL;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IRoleFunctionalityRepository _roleFunctionalityRepository;
        private readonly IFunctionalityRepository _functionalityRepository;
        private readonly IRoleMenuRepository _roleMenuRepository;

        public RoleService(IRoleRepository repository, IRoleFunctionalityRepository roleFunctionality, IFunctionalityRepository functionalityRepository, IRoleMenuRepository roleMenuRepository)
        {
            _repository = repository;
            _roleFunctionalityRepository = roleFunctionality;
            _functionalityRepository = functionalityRepository;
            _roleMenuRepository = roleMenuRepository;
        }

        public IList<Role> GetAllReadOnly()
        {
            return _repository.GetAllReadOnly();
        }

        public IList<Role> GetAllFullReadOnly()
        {
            return _repository.GetAllFullReadOnly();
        }

        public Response<Role> GetById(int id)
        {
            var response = new Response<Role>();
            var role = _repository.GetSingle(x => x.Id == id);

            if(role != null)
            {
                response.Data = role;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> Insert(Role role)
        {
            var response = new Response<Role>();

            try
            {
                role.StartDate = DateTime.Now;

                _repository.Insert(role);
                _repository.Save(string.Empty);

                response.Data = role;
                response.Messages.Add(new Message(Resources.es.Role.Created, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> Update(Role role)
        {
            var response = new Response<Role>();
            var entity = _repository.GetSingle(x => x.Id == role.Id);

            if (entity != null)
            {
                try
                {
                    if (role.Active) role.EndDate = null;

                    _repository.Update(role);
                    _repository.Save(string.Empty);
                    response.Messages.Add(new Message(Resources.es.Role.Updated, MessageType.Success));
                }
                catch (Exception)
                {
                    response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
                }
            }
            else
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            }
           
            return response;
        }

        public Response<Role> DeleteById(int id)
        {
            var response = new Response<Role>();
            var entity = _repository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                _repository.Delete(entity);
                _repository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Role.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> ChangeFunctionalities(int roleId, List<int> functionlitiesToAdd, List<int> functionlitiesToRemove)
        {
            var response = new Response<Role>();

            var roleExist = _repository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var functionalityId in functionlitiesToAdd)
                {
                    var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                    var functionalityExist = _functionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = _roleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && !roleFunctionalityExist)
                    {
                        _roleFunctionalityRepository.Insert(entity);
                    }
                }

                foreach (var functionalityId in functionlitiesToRemove)
                {
                    var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                    var functionalityExist = _functionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = _roleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && roleFunctionalityExist)
                    {
                        _roleFunctionalityRepository.Delete(entity);
                    }
                }

                _roleFunctionalityRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.RoleFunctionalitiesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> ChangeMenus(int roleId, List<int> menusToAdd, List<int> menusToRemove)
        {
            var response = new Response<Role>();

            var roleExist = _repository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var menuId in menusToAdd)
                {
                    var entity = new RoleMenu { RoleId = roleId, MenuId = menuId };
                    var menuExist = _roleMenuRepository.MenuExistById(menuId);
                    var roleMenuExist = _roleMenuRepository.ExistById(roleId, menuId);

                    if (menuExist && !roleMenuExist)
                    {
                        _roleMenuRepository.Insert(entity);
                    }
                }

                foreach (var menuId in menusToRemove)
                {
                    var entity = new RoleMenu { RoleId = roleId, MenuId = menuId };
                    var menuExist = _roleMenuRepository.MenuExistById(menuId);
                    var roleMenuExist = _roleMenuRepository.ExistById(roleId, menuId);

                    if (menuExist && roleMenuExist)
                    {
                        _roleMenuRepository.Delete(entity);
                    }
                }

                _roleMenuRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.RoleMenusUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return _repository.GetRolesByGroup(groupIds);
        }
    }
}
