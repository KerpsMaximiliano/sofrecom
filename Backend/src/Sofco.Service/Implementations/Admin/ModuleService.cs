using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Relationships;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
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
                response.Messages.Add(new Message(active ? Resources.es.Admin.Module.Enabled : Resources.es.Admin.Module.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Module.NotFound, MessageType.Error));
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

            response.Messages.Add(new Message(Resources.es.Admin.Module.NotFound, MessageType.Error));
            return response;
        }

        public Response<Module> Update(Module data)
        {
            var response = new Response<Module>();

            try
            {
                _moduleRepository.Update(data);
                _moduleRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Admin.Module.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Module> GetAllWithFunctionalitiesReadOnly()
        {
            return _moduleRepository.GetAllWithFunctionalitiesReadOnly();
        }
    }
}
