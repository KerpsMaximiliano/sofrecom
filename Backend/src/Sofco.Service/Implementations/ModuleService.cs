using Sofco.Core.Services;
using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Model.Enums;

namespace Sofco.Service.Implementations
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

                if (active)
                {
                    entity.StartDate = DateTime.Now;
                    entity.EndDate = null;
                }
                else
                {
                    entity.EndDate = DateTime.Now;
                }

                _moduleRepository.Update(entity);
                _moduleRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Module.Enabled : Resources.es.Module.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Module.NotFound, MessageType.Error));
            return response;
        }

        public IList<Module> GetAllFullReadOnly()
        {
            return _moduleRepository.GetAllFullReadOnly();
        }

        public IList<Module> GetAllReadOnly()
        {
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
    }
}
