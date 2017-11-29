using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{ 
    public class AnalyticService : IAnalyticService
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ICollection<Analytic> GetAll()
        {
            return unitOfWork.AnalyticRepository.GetAllReadOnly();
        }

        public Response<Analytic> GetById(int id)
        {
            var response = new Response<Analytic>();

            response.Data = AnalyticValidationHelper.Find(response, unitOfWork.AnalyticRepository, id);

            return response;
        }

        public Response<IList<Allocation>> GetResources(int id)
        {
            var response = new Response<IList<Allocation>>();

            var resources = unitOfWork.AnalyticRepository.GetResources(id);

            if (!resources.Any())
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.ResourcesNotFound, MessageType.Warning));
            }

            response.Data = resources;

            return response;
        }
    }
}
