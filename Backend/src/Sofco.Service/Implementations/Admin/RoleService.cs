using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Relationships;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleFunctionalityRepository _roleFunctionalityRepository;
        private readonly IFunctionalityRepository _functionalityRepository;

        public RoleService(IRoleRepository repository, IRoleFunctionalityRepository roleFunctionality, IFunctionalityRepository functionalityRepository)
        {
            _roleRepository = repository;
            _roleFunctionalityRepository = roleFunctionality;
            _functionalityRepository = functionalityRepository;
        }


        public Response<Role> Active(int id, bool active)
        {
            var response = new Response<Role>();
            var entity = _roleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                if (active)
                {
                    entity.StartDate = DateTime.Now;
                    entity.EndDate = null;
                }
                else
                {
                    entity.EndDate = DateTime.Now;
                }

                _roleRepository.Update(entity);
                _roleRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Admin.Role.Enabled : Resources.es.Admin.Role.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Role> GetAllReadOnly(bool active)
        {
            if (active)
                return _roleRepository.GetAllActivesReadOnly();
            else
                return _roleRepository.GetAllReadOnly();
        }

        public Response<Role> GetDetail(int id)
        {
            var response = new Response<Role>();
            var role = _roleRepository.GetDetail(id);

            if (role != null)
            {
                response.Data = role;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> GetById(int id)
        {
            var response = new Response<Role>();
            var role = _roleRepository.GetSingle(x => x.Id == id);

            if(role != null)
            {
                response.Data = role;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> Insert(Role role)
        {
            var response = new Response<Role>();

            try
            {
                role.StartDate = DateTime.Now;

                _roleRepository.Insert(role);
                _roleRepository.Save();

                response.Data = role;
                response.Messages.Add(new Message(Resources.es.Admin.Role.Created, MessageType.Success));
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

            try
            {
                if (role.Active) role.EndDate = null;

                _roleRepository.Update(role);
                _roleRepository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.Role.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }
           
            return response;
        }

        public Response<Role> DeleteById(int id)
        {
            var response = new Response<Role>();
            var entity = _roleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                _roleRepository.Delete(entity);
                _roleRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(Resources.es.Admin.Role.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return _roleRepository.GetRolesByGroup(groupIds);
        }

        public Response<Role> AddFunctionalities(int roleId, List<int> functionalitiesToAdd)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var functionalityId in functionalitiesToAdd)
                {
                    var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                    var functionalityExist = _functionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = _roleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && !roleFunctionalityExist)
                    {
                        _roleFunctionalityRepository.Insert(entity);
                    }
                }

                _roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.Role.ModulesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> AddFunctionality(int roleId, int functionalityId)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = _functionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Module.NotFound, MessageType.Error));
                return response;
            }

            var rolefunctionalityExist = _roleFunctionalityRepository.ExistById(roleId, functionalityId);

            if (rolefunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.RoleModuleAlreadyCreated, MessageType.Error));
            }
            else
            {
                var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                _roleFunctionalityRepository.Insert(entity);
                _roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> DeleteFunctionality(int roleId, int functionalityId)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = _functionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Module.NotFound, MessageType.Error));
                return response;
            }

            var rolefunctionalityExist = _roleFunctionalityRepository.ExistById(roleId, functionalityId);

            if (!rolefunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.RoleModuleAlreadyRemoved, MessageType.Error));
            }
            else
            {
                var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                _roleFunctionalityRepository.Delete(entity);
                _roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> RemoveFunctionalities(int roleId, List<int> functionalities)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var functionalityId in functionalities)
                {
                    var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                    var functionalityExist = _functionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = _roleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && roleFunctionalityExist)
                    {
                        _roleFunctionalityRepository.Delete(entity);
                    }
                }

                _roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.Role.ModulesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }
    }
}
