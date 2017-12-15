using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{ 
    public class AnalyticService : IAnalyticService
    {
        private readonly IAnalyticRepository analyticRepository;

        public AnalyticService(IAnalyticRepository analyticRepo)
        {
            analyticRepository = analyticRepo;
        }

        public ICollection<Analytic> GetAll()
        {
            return analyticRepository.GetAllReadOnly();
        }

        public Response<Analytic> GetById(int id)
        {
            var response = new Response<Analytic>();

            response.Data = AnalyticValidationHelper.Find(response, analyticRepository, id);

            return response;
        }

        public Response<IList<Allocation>> GetResources(int id)
        {
            var response = new Response<IList<Allocation>>();

            var resources = analyticRepository.GetResources(id);

            if (!resources.Any())
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.ResourcesNotFound, MessageType.Warning));
            }

            response.Data = resources;

            return response;
        }
    }
}
