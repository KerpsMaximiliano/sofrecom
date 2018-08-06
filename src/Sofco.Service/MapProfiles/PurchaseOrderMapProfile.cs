using System.Linq;
using AutoMapper;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Models.Reports;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Reports;

namespace Sofco.Service.MapProfiles
{
    public class PurchaseOrderMapProfile : Profile
    {
        private const char Delimiter = ';';

        public PurchaseOrderMapProfile()
        {
            CreateMap<PurchaseOrderBalanceView, PurchaseOrderBalanceViewModel>()
                .ForMember(s => s.StatusId, x => x.MapFrom(_ => _.Status))
                .ForMember(s => s.StatusText, x => x.MapFrom(_ => _.Status.ToString()))
                .ForMember(s => s.AccountManagerNames, x => x.MapFrom(_ => DistinctString(_.AccountManagerNames)))
                .ForMember(s => s.ProjectManagerNames, x => x.MapFrom(_ => DistinctString(_.ProjectManagerNames)));

            CreateMap<PurchaseOrderBalanceDetailView, PurchaseOrderBalanceDetailViewModel>()
                .ForMember(s => s.StatusId, x => x.MapFrom(_ => _.Status))
                .ForMember(s => s.StatusText, x => x.MapFrom(_ => _.Status.ToString()))
                .ForMember(s => s.Analytic, x => x.MapFrom(_ => MapAnalytic(_)));

            CreateMap<PurchaseOrder, PurchaseOrderPendingModel>()
                .ForMember(s => s.Client, x => x.MapFrom(_ => _.ClientExternalName))
                .ForMember(s => s.Area, x => x.MapFrom(_ => MapArea(_)));
        }

        private static string MapAnalytic(PurchaseOrderBalanceDetailView data)
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

        private static string DistinctString(string txtWithDelimiter)
        {
            return string.Join(Delimiter.ToString(), 
                txtWithDelimiter.Split(Delimiter).Distinct());
        }

        private static string MapArea(PurchaseOrder purchaseOrder)
        {
            return purchaseOrder.Area != null ? purchaseOrder.Area.Text : string.Empty;
        }
    }
}
