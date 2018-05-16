using System.Collections.Generic;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository repository;

        public HolidayService(IHolidayRepository repository)
        {
            this.repository = repository;
        }


        public Response<List<Holiday>> Get(int year)
        {
            var response = new Response<List<Holiday>> {Data = repository.Get(year)};

            return response;
        }

        public Response<Holiday> Post(Holiday model)
        {
            repository.Save(model);

            return new Response<Holiday> { Data = model };
        }

        public Response<List<Holiday>> ImportExternalData(int year)
        {
            throw new System.NotImplementedException();
        }
    }
}
