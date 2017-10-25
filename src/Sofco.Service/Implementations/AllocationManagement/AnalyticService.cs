using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;

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
    }
}
