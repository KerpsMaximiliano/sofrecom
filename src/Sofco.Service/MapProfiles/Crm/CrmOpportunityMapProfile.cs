using AutoMapper;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmOpportunityMapProfile : Profile
    {
        public CrmOpportunityMapProfile()
        {
            CreateMap<CrmOpportunity, Opportunity>()
                .ForMember(s => s.Id, x => x.Ignore())
                .ForMember(s => s.CrmId, x => x.MapFrom(_ => _.Id))
                .ForMember(s => s.ContactId, x => x.MapFrom(_ => _.ParentContactId));
        }
    }
}
