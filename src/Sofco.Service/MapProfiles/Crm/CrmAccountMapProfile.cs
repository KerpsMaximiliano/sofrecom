using AutoMapper;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Domain.Models.Billing;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmAccountMapProfile : Profile
    {
        public CrmAccountMapProfile()
        {
            CreateMap<CrmAccount, CrmCustomer>()
                .ForMember(s => s.Id, x => x.MapFrom(s => s.Id));

            CreateMap<Customer, CrmCustomer>()
                .ForMember(s => s.Id, x => x.MapFrom(s => s.CrmId));

            CreateMap<CrmCustomer, Customer>()
                .ForMember(s => s.Id, x => x.Ignore())
                .ForMember(s => s.CrmId, x => x.MapFrom(s => s.Id));
        }
    }
}
