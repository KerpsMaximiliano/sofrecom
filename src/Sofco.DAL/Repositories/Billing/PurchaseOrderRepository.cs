using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class PurchaseOrderRepository : BaseRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int purchaseOrderId)
        {
            return context.PurchaseOrderFiles.Any(x => x.Id == purchaseOrderId);
        }

        public PurchaseOrder GetById(int purchaseOrderId)
        {
            return context.PurchaseOrderFiles.Include(x => x.File).SingleOrDefault(x => x.Id == purchaseOrderId);
        }

        public ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters)
        {
            IQueryable<PurchaseOrder> query = context.PurchaseOrderFiles
                .Include(x => x.File)
                .Include(x => x.Analytic);

            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(parameters.ClientId) && !parameters.ClientId.Equals("0"))
                    query = query.Where(x => x.ClientExternalId.Equals(parameters.ClientId));

                if (parameters.Year > 0)
                    query = query.Where(x => x.Year == parameters.Year);

                if (!string.IsNullOrWhiteSpace(parameters.StatusId) && !parameters.StatusId.Equals("0"))
                    query = query.Where(x => x.Status == (PurchaseOrderStatus)Convert.ToInt32(parameters.StatusId));
            }

            return query.ToList();
        }

        public ICollection<PurchaseOrder> GetByService(string serviceId)
        {
            var analytic =  context.Analytics
                .Include(x => x.PurchaseOrders)
                .ThenInclude(x => x.File)
                .SingleOrDefault(x => x.ServiceId.Equals(serviceId));

            if (analytic != null && analytic.PurchaseOrders.Any())
            {
                return analytic.PurchaseOrders;
            }

            return new List<PurchaseOrder>();
        }
    }
}
