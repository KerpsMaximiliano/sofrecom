using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Framework.NolaborablesServices.Interfaces;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Nolaborables;
using Sofco.Domain.Utils;
using System;
using Sofco.Core.DAL;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository repository;
        private readonly INolaborablesService nolaborablesService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public HolidayService(IHolidayRepository repository, INolaborablesService nolaborablesService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.nolaborablesService = nolaborablesService;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public Response<List<Holiday>> Get(int year)
        {
            var response = new Response<List<Holiday>> {Data = repository.Get(year)};

            return response;
        }

        public Response<Holiday> Post(Holiday model)
        {
            var response = new Response<Holiday>();

            if (model.Date.Date <= DateTime.Now.Date)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateGreaterThanCurrent);
                return response;
            }

            repository.Save(model);
            response.AddSuccess(Resources.Common.SaveSuccess);

            //Elimino todas las horas cargadas el dia del feriado
            unitOfWork.WorkTimeRepository.RemoveAllOfDate(model.Date);
            
            response.Data = model;

            return response;
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
