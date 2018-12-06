using AutoMapper;
using Sofco.Domain.Crm.Billing;
using Sofco.Domain.Models.Billing;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmProjectMapProfile : Profile
    {
        public CrmProjectMapProfile()
        {
            CreateMap<CrmProject, Project>()
                .ForMember(s => s.Id, x => x.Ignore())
                .ForMember(s => s.CrmId, x => x.MapFrom(_ => _.Id));
        }
    }
}
