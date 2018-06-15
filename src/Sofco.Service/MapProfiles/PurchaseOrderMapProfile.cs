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
                .ForMember(s => s.StatusText, x => x.MapFrom(_ => _.Status.ToString()))
                .ForMember(s => s.Analytic, x => x.MapFrom(_ => MapAnalytic(_)));
        }

        private string MapAnalytic(PurchaseOrderBalanceDetailView data)
        {
            var analytic = data.Analytic ?? string.Empty;

            var analyticText = data.AnalyticName ?? string.Empty;

            var result = analytic;

            if (!string.IsNullOrWhiteSpace(analyticText))
            {
                result = string.Format("{0} - {1}", analytic, analyticText);
            }

            return result;
        }
    }
}
