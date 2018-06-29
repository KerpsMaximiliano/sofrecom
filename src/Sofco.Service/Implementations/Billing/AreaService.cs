using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository areaRepository;

        private readonly IMapper mapper;

        public AreaService(IAreaRepository areaRepository, IMapper mapper)
        {
            this.areaRepository = areaRepository;
            this.mapper = mapper;
        }

        public Response<List<AreaModel>> GetAll()
        {
            var result = Translate(areaRepository.GetAll());

            return new Response<List<AreaModel>>{Data = result};
        }

        private List<AreaModel> Translate(List<Area> data)
        {
            return mapper.Map<List<Area>, List<AreaModel>>(data);
        }
    }
}
