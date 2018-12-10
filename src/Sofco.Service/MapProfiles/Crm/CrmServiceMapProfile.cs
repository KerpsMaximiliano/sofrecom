using AutoMapper;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmServiceMapProfile : Profile
    {
        public CrmServiceMapProfile()
        {
            CreateMap<Domain.Models.Billing.Service, CrmService>()
                .ForMember(s => s.Id, x => x.MapFrom(s => s.CrmId));

            CreateMap<CrmService, Domain.Models.Billing.Service>()
                .ForMember(s => s.Id, x => x.Ignore())
                .ForMember(s => s.CrmId, x => x.MapFrom(s => s.Id));
        }
    }
}
