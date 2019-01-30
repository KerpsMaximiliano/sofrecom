using AutoMapper;
using Sofco.Domain.Crm;

namespace Sofco.Service.MapProfiles.Crm
{
    public class CrmInvoicingMilestoneMapProfile : Profile
    {
        public CrmInvoicingMilestoneMapProfile()
        {
            CreateMap<CrmInvoicingMilestone, CrmHito>()
                .ForMember(s => s.ScheduledDate, x => x.MapFrom(_ => _.Date));

            CreateMap<CrmInvoicingMilestone, CrmProjectHito>()
                .ForMember(s => s.Ammount, x => x.MapFrom(_ => _.Amount))
                .ForMember(s => s.StartDate, x => x.MapFrom(_ => _.Date));
        }
    }
}
