using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class SectorService : ISectorService
    {
        private readonly ISectorRepository sectorRepository;

        private readonly IMapper mapper;

        public SectorService(ISectorRepository sectorRepository, IMapper mapper)
        {
            this.sectorRepository = sectorRepository;
            this.mapper = mapper;
        }

        public Response<List<SectorModel>> GetAll()
        {
            var result = Translate(sectorRepository.GetAll());

            return new Response<List<SectorModel>> { Data = result };
        }

        private List<SectorModel> Translate(List<Sector> data)
        {
            return mapper.Map<List<Sector>, List<SectorModel>>(data);
        }
    }
}
