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

        private readonly IUserDelegateRepository userDelegateRepository;

        private readonly IMapper mapper;

        private readonly ISessionManager sessionManager;

        private readonly ILogMailer<PurchaseOrderReportService> logger;

        private readonly EmailConfig emailConfig;

        private readonly IRoleManager roleManager;

        public PurchaseOrderReportService(IPurchaseOrderBalanceViewRepository purchaseOrderRepository,
            IMapper mapper,
            ILogMailer<PurchaseOrderReportService> logger,
            IUnitOfWork unitOfWork,
            IUserData userData,
            ISessionManager sessionManager,
            IRoleManager roleManager,
            IOptions<EmailConfig> emailOptions,
            IUserDelegateRepository userDelegateRepository)
        {
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.sessionManager = sessionManager;
            this.userDelegateRepository = userDelegateRepository;
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

        public Response<List<PurchaseOrderBalanceViewModel>> GetActives(SearchPurchaseOrderParams parameters)
        {
            var response = new Response<List<PurchaseOrderBalanceViewModel>>();

            try
            {
                parameters.StatusId = string.Empty;
                parameters.StatusIds = new List<PurchaseOrderStatus> { PurchaseOrderStatus.Valid, PurchaseOrderStatus.Consumed};

                var data = purchaseOrderRepository.Search(parameters);

                data = ApplyActiveUserFilter(data);

                response.Data = Translate(data);
            }
            catch (Exception ex)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(ex);
            }

            return response;
        }

        private List<PurchaseOrderBalanceView> ApplyCurrentUserFilter(List<PurchaseOrderBalanceView> data)
        {
            var hasAllAccess = roleManager.HasFullAccess();

            if (hasAllAccess) return data;

            var currentUser = userData.GetCurrentUser();

            var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);

            return data.Where(s =>
            {
                if (string.IsNullOrEmpty(s.ManagerIds)) return false;

                var managerIds = s.ManagerIds.Split(Delimiter).Select(int.Parse).ToList();

                return analyticsByManagers.Any(_ => _.ManagerId != null && managerIds.Contains(_.ManagerId.Value));
            }).ToList();
        }

        private List<PurchaseOrderBalanceView> ApplyActiveUserFilter(List<PurchaseOrderBalanceView> data)
        {
            var hasAllAccess = roleManager.HasFullAccess();

            if (hasAllAccess) return data;

            var currentUserId = userData.GetCurrentUser().Id;

            var serviceIds = userDelegateRepository.GetByUserId(currentUserId, UserDelegateType.PurchaseOrderActive)
                .Where(s => s.ServiceId.HasValue)
                .Select(s => s.ServiceId.ToString())
                .ToList();

            var analytics = unitOfWork.AnalyticRepository.GetByServiceIds(serviceIds);

            analytics.AddRange(unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUserId));

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
