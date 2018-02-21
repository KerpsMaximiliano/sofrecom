using AutoMapper;
using Sofco.Domain.Rh.Tiger;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.MapProfiles
{
    public class HealthInsuranceMapProfile : Profile
    {
        public HealthInsuranceMapProfile()
        {
            CreateMap<TigerHealthInsurance, HealthInsurance>()
                .ForMember(d => d.Code, s => s.MapFrom(x => x.Obsoc))
                .ForMember(d => d.Name, s => s.MapFrom(x => x.Dobso.Trim()));

            CreateMap<TigerPrepaidHealth, PrepaidHealth>()
                .ForMember(d => d.HealthInsuranceCode, s => s.MapFrom(x => x.Obsoc))
                .ForMember(d => d.PrepaidHealthCode, s => s.MapFrom(x => x.Ospla))
                .ForMember(d => d.Name, s => s.MapFrom(x => x.Dospl.Trim()))
                .ForMember(d => d.Amount, s => s.MapFrom(x => x.Topes));
        }
    }
}
