using System.Collections.Generic;
using Sofco.Model.Utils;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations.Admin
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

                _functionalityRepository.Update(entity);
                _functionalityRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Admin.Functionality.Enabled : Resources.es.Admin.Functionality.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Functionality.NotFound, MessageType.Error));
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

            response.Messages.Add(new Message(Resources.es.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Functionality> GetFunctionalitiesByModule(int moduleId)
        {
            return _functionalityRepository.GetFuntionalitiesByModule(new int[] { moduleId });
        }

        public IList<Functionality> GetFunctionalitiesByModule(IEnumerable<int> modules)
        {
            return _functionalityRepository.GetFuntionalitiesByModule(modules);
        }

        public IList<RoleFunctionality> GetFunctionalitiesByRole(IEnumerable<int> roles)
        {
            return _functionalityRepository.GetFuntionalitiesByRole(roles);
        }
    }
}
