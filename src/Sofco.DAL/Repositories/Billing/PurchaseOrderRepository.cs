using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
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
            return context.PurchaseOrders
                .Include(x => x.File)
                .Include(x => x.PurchaseOrderAnalytics)
                .Include(x => x.AmmountDetails)
                    .ThenInclude(x => x.Currency)
                .SingleOrDefault(x => x.Id == purchaseOrderId);
        }

        public IList<PurchaseOrder> GetByService(string serviceId)
        {
            var ocsLite = context.PurchaseOrderAnalytics
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Analytic)
                .Where(x => x.Analytic.ServiceId.Equals(serviceId))
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
                .Where(x => x.Analytic.ServiceId.Equals(serviceId) && (x.PurchaseOrder.Status == PurchaseOrderStatus.Valid || x.PurchaseOrder.Status == PurchaseOrderStatus.Consumed))
                .Select(x => x.PurchaseOrder)
                .Distinct()
                .ToList();
        }

        public bool HasAmmountDetails(int solfacCurrencyId, int solfacPurchaseOrderId)
        {
            return context.PurchaseOrderAmmountDetails.Any(x => x.PurchaseOrderId == solfacPurchaseOrderId && x.CurrencyId == solfacCurrencyId);
        }

        public void UpdateInSolfac(int id, int solfacId)
        {
            context.Database.ExecuteSqlCommand($"UPDATE app.solfacs SET purchaseOrderId = {id} where id = {solfacId}");
        }

        public void UpdateStatus(PurchaseOrder purchaseOrder)
        {
            context.Entry(purchaseOrder).Property("Status").IsModified = true;
        }

        public void UpdateDetail(PurchaseOrderAmmountDetail detail)
        {
            context.Entry(detail).Property("Adjustment").IsModified = true;
        }

        public void UpdateAdjustment(PurchaseOrder purchaseOrder)
        {
            context.Entry(purchaseOrder).Property("Adjustment").IsModified = true;
        }

        public void AddHistory(PurchaseOrderHistory history)
        {
            context.PurchaseOrderHistories.Add(history);
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

        public IList<Analytic> GetByAnalyticsWithSectors(int purchaseOrderId)
        {
            var analyticIds = context.PurchaseOrderAnalytics
                .Where(x => x.PurchaseOrderId == purchaseOrderId)
                .Select(x => x.AnalyticId)
                .ToList();

            return context.Analytics
                .Where(x => analyticIds.Contains(x.Id))
                .Include(x => x.Sector)
                    .ThenInclude(x => x.ResponsableUser)
                .ToList();
        }

        public IList<Analytic> GetByAnalyticsWithManagers(int purchaseOrderId)
        {
            var analyticIds = context.PurchaseOrderAnalytics
                .Where(x => x.PurchaseOrderId == purchaseOrderId)
                .Select(x => x.AnalyticId)
                .ToList();

            return context.Analytics
                .Where(x => analyticIds.Contains(x.Id))
                .Include(x => x.CommercialManager)
                .Include(x => x.Manager)
                .ToList();
        }

        public ICollection<PurchaseOrderHistory> GetHistories(int id)
        {
            return context.PurchaseOrderHistories.Where(x => x.PurchaseOrderId == id).Include(x => x.User).ToList().AsReadOnly();
        }
    }
}
