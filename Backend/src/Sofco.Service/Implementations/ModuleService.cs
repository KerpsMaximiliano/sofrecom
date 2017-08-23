using Sofco.Core.Services;
using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Model.Enums;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IFunctionalityRepository _functionalityRepository;
        private readonly IModuleFunctionalityRepository _moduleFunctionalityRepository;
        private readonly IRoleModuleRepository _roleModuleRepository;

        public ModuleService(IModuleRepository moduleRepository, 
            IFunctionalityRepository functionalityRepository, 
            IModuleFunctionalityRepository moduleFunctionalityRepository,
            IRoleModuleRepository roleModuleRepository)
        {
            _moduleRepository = moduleRepository;
            _functionalityRepository = functionalityRepository;
            _moduleFunctionalityRepository = moduleFunctionalityRepository;
            _roleModuleRepository = roleModuleRepository;
        }

        public Response<Module> Active(int id, bool active)
        {
            var response = new Response<Module>();
            var entity = _moduleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                _moduleRepository.Update(entity);
                _moduleRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Module.Enabled : Resources.es.Module.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
            return response;
        }

        public IList<Module> GetAllReadOnly(bool active)
        {
            if (active)
                return _moduleRepository.GetAllActivesReadOnly();
            else
                return _moduleRepository.GetAllReadOnly();
        }

        public Response<Module> GetById(int id)
        {
            var response = new Response<Module>();
            var entity = _moduleRepository.GetSingleWithFunctionalities(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
            return response;
        }

        public Response<Module> Update(Module data)
        {
            var response = new Response<Module>();

            try
            {
                _moduleRepository.Update(data);
                _moduleRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Module.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Module> ChangeFunctionalities(int moduleId, List<int> functionlitiesToAdd)
        {
            var response = new Response<Module>();

            var moduleExist = _moduleRepository.ExistById(moduleId);

            if (!moduleExist)
            {
                response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var functionalityId in functionlitiesToAdd)
                {
                    var entity = new ModuleFunctionality { ModuleId = moduleId, FunctionalityId = functionalityId };
                    var functionalityExist = _functionalityRepository.ExistById(functionalityId);
                    var ModuleFunctionalityExist = _moduleFunctionalityRepository.ExistById(moduleId, functionalityId);

                    if (functionalityExist && !ModuleFunctionalityExist)
                    {
                        _moduleFunctionalityRepository.Insert(entity);
                    }
                }

                _moduleFunctionalityRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Module.FunctionalitiesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Functionality> AddFunctionality(int moduleId, int functionalityId)
        {
            var response = new Response<Functionality>();

            var moduleExist = _moduleRepository.ExistById(moduleId);

            if (!moduleExist)
            {
                response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = _functionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
                return response;
            }

            var moduleFunctionalityExist = _moduleFunctionalityRepository.ExistById(moduleId, functionalityId);

            if (moduleFunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Functionality.ModuleFunctionalityAlreadyCreated, MessageType.Error));
            }
            else
            {
                var entity = new ModuleFunctionality { ModuleId = moduleId, FunctionalityId = functionalityId };
                _moduleFunctionalityRepository.Insert(entity);
                _moduleFunctionalityRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Module.FunctionalitiesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Functionality> DeleteFunctionality(int moduleId, int functionalityId)
        {
            var response = new Response<Functionality>();

            var moduleExist = _moduleRepository.ExistById(moduleId);

            if (!moduleExist)
            {
                response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = _functionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
                return response;
            }

            var roleModuleFunctionalityExist = _moduleFunctionalityRepository.ExistById(moduleId, functionalityId);

            if (!roleModuleFunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.es.Functionality.ModuleFunctionalityAlreadyRemoved, MessageType.Error));
            }
            else
            {
                var entity = new ModuleFunctionality { ModuleId = moduleId, FunctionalityId = functionalityId };
                _moduleFunctionalityRepository.Delete(entity);
                _moduleFunctionalityRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Role.RoleFunctionalitiesUpdated, MessageType.Success));
            }

            return response;
        }

        public IList<Module> GetModulesByRole(IEnumerable<int> roles)
        {
            return _roleModuleRepository.GetModulesByRoles(roles);
        }
    }
}
