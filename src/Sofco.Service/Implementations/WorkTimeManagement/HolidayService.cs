using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Framework.NolaborablesServices.Interfaces;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Nolaborables;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository repository;

        private readonly INolaborablesService nolaborablesService;

        private readonly IMapper mapper;

        public HolidayService(IHolidayRepository repository, INolaborablesService nolaborablesService, IMapper mapper)
        {
            this.repository = repository;
            this.nolaborablesService = nolaborablesService;
            this.mapper = mapper;
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
            var holidays = Translate(nolaborablesService.Get(year).Data);

            repository.SaveFromExternalData(holidays);

            return new Response<List<Holiday>>{ Data = holidays };
        }

        public Response Delete(int holidayId)
        {
            repository.Delete(holidayId);

            return new Response();
        }

        private List<Holiday> Translate(List<Feriado> feriados)
        {
            return mapper.Map<List<Feriado>, List<Holiday>>(feriados);
        }
    }
}
