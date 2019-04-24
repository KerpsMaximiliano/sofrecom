using AutoMapper;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmContactMapProfile : Profile
    {
        public CrmContactMapProfile() 
        {
            CreateMap<CrmContact, Contact>()
                .ForMember(s => s.Id, x => x.Ignore())
                .ForMember(s => s.CrmId, x => x.MapFrom(_ => _.Id));
        }
    }
}
