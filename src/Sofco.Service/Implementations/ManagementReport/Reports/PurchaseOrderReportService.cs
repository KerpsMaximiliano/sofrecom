using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.Views;
using Sofco.Core.Logger;
using Sofco.Core.Models.Reports;
using Sofco.Core.Services.Reports;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Reports;
using Sofco.Domain.Utils;
using Sofco.Core.Managers;

namespace Sofco.Service.Implementations.Reports
{
    public class PurchaseOrderReportService : IPurchaseOrderReportService
    {
        private const char Delimiter = ';';

        private const int PdfPathId = 1;

        private readonly IUnitOfWork unitOfWork;

        private readonly IPurchaseOrderBalanceViewRepository purchaseOrderRepository;

        private readonly IUserData userData;

        private readonly IMapper mapper;

        private readonly ILogMailer<PurchaseOrderReportService> logger;

        private readonly EmailConfig emailConfig;

        private readonly IRoleManager roleManager;

        public PurchaseOrderReportService(IPurchaseOrderBalanceViewRepository purchaseOrderRepository,
            IMapper mapper,
            ILogMailer<PurchaseOrderReportService> logger,
            IUnitOfWork unitOfWork,
            IUserData userData,
            IRoleManager roleManager,
            IOptions<EmailConfig> emailOptions)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.emailConfig = emailOptions.Value;
            this.roleManager = roleManager;
        }

        public Response<List<PurchaseOrderBalanceViewModel>> Get(SearchPurchaseOrderParams parameters)
        {
            var response = new Response<List<PurchaseOrderBalanceViewModel>>();

            try
            {
                var data = purchaseOrderRepository.Search(parameters);

                data = ApplyCurrentUserFilter(data);

                response.Data = Translate(data);
            }
            catch (Exception ex)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(ex);
            }

            return response;
        }

        public Response<List<Option>> GetAnalyticsByCurrentUser()
        {
            var hasAllAccess = roleManager.HasFullAccess(); 

            List<Analytic> analytics;

            if (hasAllAccess)
            {
                analytics = unitOfWork.AnalyticRepository.GetAllReadOnly().ToList();
            }
            else
            {
                var currentUser = userData.GetCurrentUser();

                analytics = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id).ToList();
            }

            var result = analytics.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();

            return new Response<List<Option>> { Data = result };
        }

        private List<PurchaseOrderBalanceView> ApplyCurrentUserFilter(List<PurchaseOrderBalanceView> data)
        {
            var hasAllAccess = roleManager.HasFullAccess();

            if (hasAllAccess) return data;

            var currentUserId = userData.GetCurrentUser().Id;

            var delegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUserId, DelegationType.PurchaseOrderActive);

            var analytics = unitOfWork.AnalyticRepository.GetActiveByIds(delegates.Select(x => x.AnalyticSourceId.GetValueOrDefault()).ToList());

            var analyticsByManager = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUserId);

            foreach (var analytic in analyticsByManager)
            {
                analytics.Add(analytic);
            }

            return data.Where(s =>
            {
                if (string.IsNullOrEmpty(s.ManagerIds)) return false;

                var managerIds = s.ManagerIds.Split(Delimiter).Select(int.Parse).ToList();

                return analytics.Any(_ => _.ManagerId != null && managerIds.Contains(_.ManagerId.Value));
            }).ToList();
        }

        private List<PurchaseOrderBalanceViewModel> Translate(List<PurchaseOrderBalanceView> data)
        {
            var result = mapper.Map<List<PurchaseOrderBalanceView>, List<PurchaseOrderBalanceViewModel>>(data);

            var details =
                purchaseOrderRepository.GetByPurchaseOrderIds(result.Select(s => s.PurchaseOrderId).ToList());

            foreach (var item in result)
            {
                if(item.FileId.HasValue && item.FileId.Value > 0)
                    item.PdfUrl = $"{emailConfig.SiteUrl}pdf/{item.FileId}/{PdfPathId}";

                item.Details = Translate(details.Where(s =>
                    s.PurchaseOrderId == item.PurchaseOrderId
                    && s.CurrencyId == item.CurrencyId).ToList());
            }
            return result;
        }

        private List<PurchaseOrderBalanceDetailViewModel> Translate(List<PurchaseOrderBalanceDetailView> data)
        {
            return mapper.Map<List<PurchaseOrderBalanceDetailView>, List<PurchaseOrderBalanceDetailViewModel>>(data);
        }
    }
}
