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
    public class FunctionalityService : IFunctionalityService
    {
        private readonly IFunctionalityRepository _functionalityRepository;
        private readonly IModuleFunctionalityRepository _moduleFunctionalityRepository;

        public FunctionalityService(IFunctionalityRepository functionalityRepository, IModuleFunctionalityRepository moduleFunctionalityRepository)
        {
            _functionalityRepository = functionalityRepository;
            _moduleFunctionalityRepository = moduleFunctionalityRepository;
        }

        public Response<Functionality> Active(int id, bool active)
        {
            var response = new Response<Functionality>();
            var entity = _functionalityRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                _functionalityRepository.Update(entity);
                _functionalityRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Functionality.Enabled : Resources.es.Functionality.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Functionality> GetAllReadOnly(bool active)
        {
            if (active)
                return _functionalityRepository.GetAllActivesReadOnly();
            else
                return _functionalityRepository.GetAllReadOnly();
        }


        public Response<Functionality> GetById(int id)
        {
            var response = new Response<Functionality>();
            var entity = _functionalityRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Functionality> GetFunctionalitiesByModule(int moduleId)
        {
            return _moduleFunctionalityRepository.GetFuntionalitiesByModule(new int[] { moduleId });
        }

        public IList<Functionality> GetFunctionalitiesByModule(IEnumerable<int> modules)
        {
            return _moduleFunctionalityRepository.GetFuntionalitiesByModule(modules);
        }
    }
}
