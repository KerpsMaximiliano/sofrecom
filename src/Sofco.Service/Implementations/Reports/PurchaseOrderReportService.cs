using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Views;
using Sofco.Core.Logger;
using Sofco.Core.Models.Reports;
using Sofco.Core.Services.Reports;
using Sofco.Model.DTO;
using Sofco.Model.Models.Reports;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Reports
{
    public class PurchaseOrderReportService : IPurchaseOrderReportService
    {
        private const char Delimiter = ';';

        private readonly IUnitOfWork unitOfWork;

        private readonly IPurchaseOrderBalanceViewRepository purchaseOrderRepository;

        private readonly IUserData userData;

        private readonly IMapper mapper;

        private readonly ILogMailer<PurchaseOrderReportService> logger;

        public PurchaseOrderReportService(IPurchaseOrderBalanceViewRepository purchaseOrderRepository, IMapper mapper, ILogMailer<PurchaseOrderReportService> logger, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public Response<List<PurchaseOrderBalanceViewModel>> Get(SearchPurchaseOrderParams parameters)
        {
            var response = new Response<List<PurchaseOrderBalanceViewModel>>();

            try
            {
                var data = purchaseOrderRepository.Search(parameters);

                data = ApplyCurrentManagerIdFilter(data);

                var result = Translate(data);

                var details =
                    purchaseOrderRepository.GetByPurchaseOrderIds(result.Select(s => s.PurchaseOrderId).ToList());

                foreach (var item in result)
                {
                    item.Details = Translate(details.Where(s =>
                        s.PurchaseOrderId == item.PurchaseOrderId 
                        && s.CurrencyId == item.CurrencyId).ToList());
                }

                response.Data = result;
            }
            catch (Exception ex)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(ex);
            }

            return response;
        }

        private List<PurchaseOrderBalanceView> ApplyCurrentManagerIdFilter(List<PurchaseOrderBalanceView> data)
        {
            var currentUser = userData.GetCurrentUser();

            var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagers(currentUser.Id);

            return data.Where(s =>
            {
                if (string.IsNullOrEmpty(s.ManagerIds)) return false;

                var managerIds = s.ManagerIds.Split(Delimiter).Select(int.Parse).ToList();

                return analyticsByManagers.Any(_ => _.ManagerId != null && managerIds.Contains(_.ManagerId.Value));
            }).ToList();
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
