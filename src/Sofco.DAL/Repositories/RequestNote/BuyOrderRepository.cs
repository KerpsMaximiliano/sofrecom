using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.RequestNote;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Models.RequestNote;
using Sofco.DAL.Repositories.Common;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class BuyOrderRepository : BaseRepository<Sofco.Domain.Models.RequestNote.BuyOrder>, IBuyOrderRepository
    {
        protected readonly SofcoContext context;

        public BuyOrderRepository(SofcoContext context) : base(context)
        {
            this.context = context;
        }

        public Domain.Models.RequestNote.BuyOrder GetById(int id)
        {
            return this.context.BuyOrders.Where(x => x.Id == id)
                .Include(x=> x.RequestNote)
                    .ThenInclude(p=> p.ProductsServices)
                .Include(x => x.ProductsServices)
                    .ThenInclude(p=> p.RequestNoteProductService)
                .Include(x => x.Invoices)
                    .ThenInclude(p => p.File)
                .Include(x => x.Invoices)
                    .ThenInclude(p => p.ProductsServices)
                .Include(x => x.Workflow)
                .Include(x => x.Status)
                .Include(x => x.CreationUser)
                .Include(x => x.UserApplicant)
                .Include(x=> x.Histories)
                .Include(x => x.Provider)
                    .ThenInclude(z => z.Provider)
                .FirstOrDefault();
        }

        public IList<Domain.Models.RequestNote.BuyOrder> GetAll(BuyOrderGridFilters filters)
        {
            if (filters == null)
                filters = new BuyOrderGridFilters();
            if (filters.FromDate.HasValue)
                filters.FromDate = filters.FromDate.Value.Date;
            if (filters.ToDate.HasValue)
                filters.ToDate = filters.ToDate.Value.Date.AddDays(1);

            return this.context.BuyOrders
                .Where(n=> !filters.Id.HasValue || n.Id == filters.Id)
                .Where(n => !string.IsNullOrEmpty(filters.Number) || n.BuyOrderNumber == filters.Number)
                .Where(n=> !filters.StatusId.HasValue || n.StatusId == filters.StatusId)
                .Where(n=> !filters.FromDate.HasValue || n.CreationDate >= filters.FromDate)
                .Where(n => !filters.ToDate.HasValue || n.CreationDate < filters.ToDate)
                .Where(n => !filters.RequestNoteId.HasValue || n.RequestNoteId == filters.RequestNoteId)
                .Where(n => !filters.ProviderId.HasValue || n.ProviderId == filters.ProviderId)
                .Include(x => x.Status)
                .Include(x => x.ProductsServices)
                .Include(x => x.RequestNote)
                .Include(x => x.Provider)
                    .ThenInclude(z => z.Provider)
                .ToList();
        }

        public void UpdateBuyOrder(Domain.Models.RequestNote.BuyOrder buyOrder)
        {
            this.context.BuyOrders.Update(buyOrder);
        }

        public void InsertBuyOrder(Domain.Models.RequestNote.BuyOrder buyOrder)
        {
            this.context.BuyOrders.Add(buyOrder);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }   
}
