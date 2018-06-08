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
            }

            return query.ToList();
        }

        public List<PurchaseOrderBalanceDetailView> GetByPurchaseOrderIds(List<int> purchaseOrderIds)
        {
            IQueryable<PurchaseOrderBalanceDetailView> query = purchaseOrderBalanceDetailViews
                .Where(s => purchaseOrderIds.Contains(s.PurchaseOrderId));

            return query.ToList();
        }
    }
}
