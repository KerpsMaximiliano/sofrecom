using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Views;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Reports;

namespace Sofco.DAL.Repositories.Reports
{
    public class PurchaseOrderBalanceViewRepository : IPurchaseOrderBalanceViewRepository
    {
        private readonly DbSet<PurchaseOrderBalanceView> purchaseOrderBalanceViews;

        private readonly DbSet<PurchaseOrderBalanceDetailView> purchaseOrderBalanceDetailViews;

        public PurchaseOrderBalanceViewRepository(ReportContext context)
        {
            purchaseOrderBalanceViews = context.Set<PurchaseOrderBalanceView>();

            purchaseOrderBalanceDetailViews = context.Set<PurchaseOrderBalanceDetailView>();
        }

        public List<PurchaseOrderBalanceView> Search(SearchPurchaseOrderParams parameters)
        {
            IQueryable<PurchaseOrderBalanceView> query = purchaseOrderBalanceViews;

            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                    query = query.Where(x => x.ClientExternalId.Equals(parameters.ClientId));

                if (!string.IsNullOrWhiteSpace(parameters.StatusId) && !parameters.StatusId.Equals("0"))
                    query = query.Where(x => x.Status == (PurchaseOrderStatus)Convert.ToInt32(parameters.StatusId));

                if (parameters.StartDate.HasValue && parameters.StartDate != DateTime.MinValue)
                    query = query.Where(x => x.ReceptionDate >= parameters.StartDate);

                if (parameters.EndDate.HasValue && parameters.EndDate != DateTime.MinValue)
                    query = query.Where(x => x.ReceptionDate <= parameters.EndDate);
            }

            var result = query.ToList();

            if (parameters == null) return result;

            if (parameters.AnalyticId.HasValue)
            {
                result = result.Where(s =>
                {
                    var analyticIds = s.AnalyticIds.Split(',').Select(int.Parse).Distinct();
                    return analyticIds.Contains(parameters.AnalyticId.Value);
                }).ToList();
            }

            if (parameters.ManagerId.HasValue)
            {
                result = result.Where(s =>
                {
                    var managerIds = s.ManagerIds.Split(',').Select(int.Parse).Distinct();
                    return managerIds.Contains(parameters.ManagerId.Value);
                }).ToList();
            }

            if (parameters.CommercialManagerId.HasValue)
            {
                result = result.Where(s =>
                {
                    var commercialManagerIds = s.CommercialManagerIds.Split(',').Select(int.Parse).Distinct();
                    return commercialManagerIds.Contains(parameters.CommercialManagerId.Value);
                }).ToList();
            }

            return result;
        }

        public List<PurchaseOrderBalanceDetailView> GetByPurchaseOrderIds(List<int> purchaseOrderIds)
        {
            IQueryable<PurchaseOrderBalanceDetailView> query = purchaseOrderBalanceDetailViews
                .Where(s => purchaseOrderIds.Contains(s.PurchaseOrderId));

            return query.ToList();
        }
    }
}
