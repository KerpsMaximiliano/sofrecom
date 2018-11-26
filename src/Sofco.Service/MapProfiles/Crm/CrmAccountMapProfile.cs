using AutoMapper;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmAccountMapProfile : Profile
    {
        public CrmAccountMapProfile()
        {
            CreateMap<CrmAccount, CrmCustomer>()
                .ForMember(s => s.Id, x => x.MapFrom(s => s.Id));
        }
    }
}
