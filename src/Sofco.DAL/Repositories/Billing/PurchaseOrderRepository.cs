using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Billing
{
    public class PurchaseOrderRepository : BaseRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int purchaseOrderId)
        {
            return context.PurchaseOrders.Any(x => x.Id == purchaseOrderId);
        }

        public PurchaseOrder GetById(int purchaseOrderId)
        {
            return context.PurchaseOrders
                .Include(x => x.File)
                .Include(x => x.AmmountDetails)
                    .ThenInclude(x => x.Currency)
                .SingleOrDefault(x => x.Id == purchaseOrderId);
        }

        public PurchaseOrder GetWithAnalyticsById(int purchaseOrderId)
        {
            return context.PurchaseOrders.Include(x => x.File).Include(x => x.PurchaseOrderAnalytics).SingleOrDefault(x => x.Id == purchaseOrderId);
        }

        public IList<PurchaseOrder> GetByService(string serviceId)
        {
            var ocsLite = context.PurchaseOrderAnalytics
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Analytic)
                .Where(x => x.Analytic.ServiceId.Equals(serviceId) && x.PurchaseOrder.Status == PurchaseOrderStatus.Valid)
                .Select(x => x.PurchaseOrderId)
                .Distinct()
                .ToList();

            return context.PurchaseOrders
                .Include(x => x.File)
                .Include(x => x.AmmountDetails)
                    .ThenInclude(x => x.Currency)
                .Where(x => ocsLite.Contains(x.Id))
                .ToList();
        }

        public IList<PurchaseOrder> GetByServiceLite(string serviceId)
        {
            return context.PurchaseOrderAnalytics
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Analytic)
                .Where(x => x.Analytic.ServiceId.Equals(serviceId) && x.PurchaseOrder.Status == PurchaseOrderStatus.Valid)
                .Select(x => x.PurchaseOrder)
                .Distinct()
                .ToList();
        }

        public void UpdateBalance(PurchaseOrderAmmountDetail detail)
        {
            context.Entry(detail).Property("Balance").IsModified = true;
        }

        public ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters)
        {
            IQueryable<PurchaseOrder> query = context.PurchaseOrders
                .Include(x => x.AmmountDetails)
                    .ThenInclude(x => x.Currency)
                .Include(x => x.File);

            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                    query = query.Where(x => x.ClientExternalId.Equals(parameters.ClientId));

                if (!string.IsNullOrWhiteSpace(parameters.StatusId) && !parameters.StatusId.Equals("0"))
                    query = query.Where(x => x.Status == (PurchaseOrderStatus)Convert.ToInt32(parameters.StatusId));
            }

            return query.ToList();
        }

        public void AddPurchaseOrderAnalytic(PurchaseOrderAnalytic purchaseOrderAnalytic)
        {
            context.PurchaseOrderAnalytics.Add(purchaseOrderAnalytic);
        }
    }
}
