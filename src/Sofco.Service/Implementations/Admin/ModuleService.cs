using System;
using System.Collections.Generic;
using Sofco.Model.Utils;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository moduleRepository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            this.moduleRepository = moduleRepository;
        }

        public Response<Module> Active(int id, bool active)
        {
            var response = new Response<Module>();
            var entity = moduleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                moduleRepository.Update(entity);
                moduleRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.Admin.Module.Enabled : Resources.Admin.Module.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
            return response;
        }

        public IList<Module> GetAllReadOnly(bool active)
        {
            if (active)
                return moduleRepository.GetAllActivesReadOnly();
            else
                return moduleRepository.GetAllReadOnly();
        }

        public Response<Module> GetById(int id)
        {
            var response = new Response<Module>();
            var entity = moduleRepository.GetSingleWithFunctionalities(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
            return response;
        }

        public Response<Module> Update(Module data)
        {
            var response = new Response<Module>();

            try
            {
                moduleRepository.Update(data);
                moduleRepository.Save();
                response.Messages.Add(new Message(Resources.Admin.Module.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Module> GetAllWithFunctionalitiesReadOnly()
        {
            return moduleRepository.GetAllWithFunctionalitiesReadOnly();
        }
    }
}
