using AutoMapper;
using Sofco.Core.Models.Reports;
using Sofco.Model.Models.Reports;

namespace Sofco.Service.MapProfiles
{
    public class PurchaseOrderMapProfile : Profile
    {
        public PurchaseOrderMapProfile()
        {
            CreateMap<PurchaseOrderBalanceView, PurchaseOrderBalanceViewModel>()
                .ForMember(s => s.StatusId, x => x.MapFrom(_ => _.Status))
                .ForMember(s => s.StatusText, x => x.MapFrom(_ => _.Status.ToString()));


            CreateMap<PurchaseOrderBalanceDetailView, PurchaseOrderBalanceDetailViewModel>()
                .ForMember(s => s.StatusId, x => x.MapFrom(_ => _.Status))
                .ForMember(s => s.StatusText, x => x.MapFrom(_ => _.Status.ToString()));
        }
    }
}
