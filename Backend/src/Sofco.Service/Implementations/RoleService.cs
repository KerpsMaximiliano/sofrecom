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
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleModuleRepository _roleModuleRepository;
        private readonly IFunctionalityRepository _functionalityRepository;
        private readonly IModuleRepository _moduleRepository;

        public RoleService(IRoleRepository repository, IRoleModuleRepository roleFunctionality, IFunctionalityRepository functionalityRepository, IModuleRepository moduleRepository)
        {
            _roleRepository = repository;
            _roleModuleRepository = roleFunctionality;
            _functionalityRepository = functionalityRepository;
            _moduleRepository = moduleRepository;
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
                _roleRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Role.Enabled : Resources.es.Role.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
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

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
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

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> Insert(Role role)
        {
            var response = new Response<Role>();

            try
            {
                role.StartDate = DateTime.Now;

                _roleRepository.Insert(role);
                _roleRepository.Save(string.Empty);

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

            try
            {
                if (role.Active) role.EndDate = null;

                _roleRepository.Update(role);
                _roleRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.Updated, MessageType.Success));
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
                _roleRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(Resources.es.Role.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            return response;
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return _roleRepository.GetRolesByGroup(groupIds);
        }

        public Response<Role> ChangeModules(int roleId, List<int> modulesToAdd)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var moduleId in modulesToAdd)
                {
                    var entity = new RoleModule { RoleId = roleId, ModuleId = moduleId };
                    var moduleExist = _moduleRepository.ExistById(moduleId);
                    var roleModuleExist = _roleModuleRepository.ExistById(roleId, moduleId);

                    if (moduleExist && !roleModuleExist)
                    {
                        _roleModuleRepository.Insert(entity);
                    }
                }

                _roleModuleRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.ModulesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> AddModule(int roleId, int moduleId)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            var moduleExist = _moduleRepository.ExistById(moduleId);

            if (!moduleExist)
            {
                response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
                return response;
            }

            var roleModuleExist = _roleModuleRepository.ExistById(roleId, moduleId);

            if (roleModuleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.RoleModuleAlreadyCreated, MessageType.Error));
            }
            else
            {
                var entity = new RoleModule { RoleId = roleId, ModuleId = moduleId };
                _roleModuleRepository.Insert(entity);
                _roleModuleRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> DeleteModule(int roleId, int moduleId)
        {
            var response = new Response<Role>();

            var roleExist = _roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            var moduleExist = _moduleRepository.ExistById(moduleId);

            if (!moduleExist)
            {
                response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
                return response;
            }

            var roleModuleExist = _roleModuleRepository.ExistById(roleId, moduleId);

            if (!roleModuleExist)
            {
                response.Messages.Add(new Message(Resources.es.Role.RoleModuleAlreadyRemoved, MessageType.Error));
            }
            else
            {
                var entity = new RoleModule { RoleId = roleId, ModuleId = moduleId };
                _roleModuleRepository.Delete(entity);
                _roleModuleRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }
    }
}
