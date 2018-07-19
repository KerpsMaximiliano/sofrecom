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
        private const char Delimiter = ';';

        private const int Limit = 1000;

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

            if (!IsValidParameters(parameters)) return query.Take(Limit).ToList();

            if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                query = query.Where(x => x.ClientExternalId.Equals(parameters.ClientId));

            if (!string.IsNullOrWhiteSpace(parameters.StatusId) && !parameters.StatusId.Equals("0"))
                query = query.Where(x => x.Status == (PurchaseOrderStatus)Convert.ToInt32(parameters.StatusId));

            if (parameters.StatusIds != null && parameters.StatusIds.Count > 0)
            {
                query = query.Where(x => parameters.StatusIds.Contains(x.Status));
            }

            if (parameters.StartDate.HasValue && parameters.StartDate != DateTime.MinValue)
                query = query.Where(x => x.ReceptionDate >= parameters.StartDate);

            if (parameters.EndDate.HasValue && parameters.EndDate != DateTime.MinValue)
                query = query.Where(x => x.ReceptionDate <= parameters.EndDate);

            var result = query.ToList();

            if (parameters.AnalyticId.HasValue)
            {
                result = result.Where(s =>
                {
                    if (s.AnalyticIds == null) return false;

                    var analyticIds = s.AnalyticIds.Split(Delimiter).Select(int.Parse).Distinct();

                    return analyticIds.Contains(parameters.AnalyticId.Value);
                }).ToList();
            }

            if (parameters.ManagerId.HasValue)
            {
                result = result.Where(s =>
                {
                    if (s.ManagerIds == null) return false;

                    var managerIds = s.ManagerIds.Split(Delimiter).Select(int.Parse).Distinct();

                    return managerIds.Contains(parameters.ManagerId.Value);
                }).ToList();
            }

            if (parameters.CommercialManagerId.HasValue)
            {
                result = result.Where(s =>
                {
                    if (s.CommercialManagerIds == null) return false;

                    var commercialManagerIds = s.CommercialManagerIds.Split(Delimiter).Select(int.Parse).Distinct();

                    return commercialManagerIds.Contains(parameters.CommercialManagerId.Value);
                }).ToList();
            }

            return result;
        }

        public List<PurchaseOrderBalanceDetailView> GetByPurchaseOrderIds(List<int> purchaseOrderIds)
        {
            var query = purchaseOrderBalanceDetailViews
                .Where(s => purchaseOrderIds.Contains(s.PurchaseOrderId));

            return query.ToList();
        }

        private bool IsValidParameters(SearchPurchaseOrderParams parameters)
        {
            if (parameters == null) return false;

            if (parameters.AnalyticId == null
                && parameters.ClientId == null
                && parameters.StatusId == null
                && parameters.CommercialManagerId == null
                && parameters.ManagerId == null
                && parameters.StartDate == null
                && parameters.EndDate == null)
            {
                return false;
            }

            return true;
        }
    }
}
