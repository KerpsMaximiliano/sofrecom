using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Core.Services.Admin;
using Sofco.DAL;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations.Admin
{
    public class FunctionalityService : IFunctionalityService
    {
        private readonly IUnitOfWork unitOfWork;

        public FunctionalityService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Response<Functionality> Active(int id, bool active)
        {
            var response = new Response<Functionality>();
            var entity = unitOfWork.FunctionalityRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                unitOfWork.FunctionalityRepository.Update(entity);
                unitOfWork.FunctionalityRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.Admin.Functionality.Enabled : Resources.Admin.Functionality.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Functionality> GetAllReadOnly(bool active)
        {
            if (active)
                return unitOfWork.FunctionalityRepository.GetAllActivesReadOnly();
            else
                return unitOfWork.FunctionalityRepository.GetAllReadOnly();
        }


        public Response<Functionality> GetById(int id)
        {
            var response = new Response<Functionality>();
            var entity = unitOfWork.FunctionalityRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Functionality> GetFunctionalitiesByModule(int moduleId)
        {
            return unitOfWork.FunctionalityRepository.GetFuntionalitiesByModule(new int[] { moduleId });
        }

        public IList<Functionality> GetFunctionalitiesByModule(IEnumerable<int> modules)
        {
            return unitOfWork.FunctionalityRepository.GetFuntionalitiesByModule(modules);
        }

        public IList<RoleFunctionality> GetFunctionalitiesByRole(IEnumerable<int> roles)
        {
            return unitOfWork.FunctionalityRepository.GetFuntionalitiesByRole(roles);
        }
    }
}
