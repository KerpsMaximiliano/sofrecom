using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.DAL.Views;
using Sofco.Core.Models.Reports;
using Sofco.Core.Services.Reports;
using Sofco.Model.DTO;
using Sofco.Model.Models.Reports;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Reports
{
    public class PurchaseOrderReportService : IPurchaseOrderReportService
    {
        private readonly IPurchaseOrderBalanceViewRepository purchaseOrderRepository;

        private readonly IMapper mapper;

        public PurchaseOrderReportService(IPurchaseOrderBalanceViewRepository purchaseOrderRepository, IMapper mapper)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.mapper = mapper;
        }

        public Response<List<PurchaseOrderBalanceViewModel>> Get(SearchPurchaseOrderParams parameters)
        {
            var result = Translate(purchaseOrderRepository.Search(parameters));

            var response = new Response<List<PurchaseOrderBalanceViewModel>>
            {
                Data = result
            };

            var details = purchaseOrderRepository.GetByPurchaseOrderIds(result.Select(s => s.PurchaseOrderId).ToList());

            foreach (var item in result)
            {
                item.Details = Translate(details.Where(s => s.PurchaseOrderId == item.PurchaseOrderId && s.CurrencyId == item.CurrencyId).ToList());
            }

            return response;
        }

        private List<PurchaseOrderBalanceViewModel> Translate(List<PurchaseOrderBalanceView> data)
        {
            return mapper.Map<List<PurchaseOrderBalanceView>, List<PurchaseOrderBalanceViewModel>>(data);
        }

        private List<PurchaseOrderBalanceDetailViewModel> Translate(List<PurchaseOrderBalanceDetailView> data)
        {
            return mapper.Map<List<PurchaseOrderBalanceDetailView>, List<PurchaseOrderBalanceDetailViewModel>>(data);
        }
    }
}
