using Sofco.Core.Services;
using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Model.Enums;

namespace Sofco.Service.Implementations
{
    public class FunctionalityService : IFunctionalityService
    {
        private readonly IFunctionalityRepository _functionalityRepository;

        public FunctionalityService(IFunctionalityRepository functionalityRepository)
        {
            _functionalityRepository = functionalityRepository;
        }

        public Response<Functionality> Active(int id, bool active)
        {
            var response = new Response<Functionality>();
            var entity = _functionalityRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                if (active)
                    entity.StartDate = DateTime.Now;
                else
                    entity.EndDate = DateTime.Now;

                _functionalityRepository.Update(entity);
                _functionalityRepository.Save(string.Empty);

                response.Messages.Add(new Message(active ? Resources.es.Functionality.Enabled : Resources.es.Functionality.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Functionality> GetAllReadOnly()
        {
            return _functionalityRepository.GetAllReadOnly();
        }

        public Response<Functionality> GetById(int id)
        {
            var response = new Response<Functionality>();
            var entity = _functionalityRepository.GetSingleWithRoles(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
            return response;
        }
    }
}
