﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.RequestNote;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.RequestNote;
using Sofco.DAL.Repositories.Common;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteRepository : BaseRepository<Sofco.Domain.Models.RequestNote.RequestNote>, IRequestNoteRepository
    {
        protected readonly SofcoContext context;

        public RequestNoteRepository(SofcoContext context) : base(context)
        {
            this.context = context;
        }

        public Domain.Models.RequestNote.RequestNote GetById(int id)
        {
            return this.context.RequestNote.Where(x => x.Id == id)
                .Include(x => x.ProviderArea)
                .Include(x => x.Attachments)
                    .ThenInclude(p => p.File)
                .Include(x => x.ProductsServices)
                .Include(x => x.Providers)
                    .ThenInclude(p => p.File)
                .Include(x => x.Providers)
                    .ThenInclude(p => p.Provider)
                //.Include(x => x.ProvidersSugg)
                //    .ThenInclude(p => p.Provider)
                //.Include(x => x.ProvidersSugg)
                //    .ThenInclude(p => p.File)
                .Include(x => x.Analytics)
                    .ThenInclude(p => p.Analytic)
                .Include(x => x.Trainings)
                    .ThenInclude(e => e.Employees)
                        .ThenInclude(n => n.Employee)
                .Include(x => x.Travels)
                    .ThenInclude(e => e.Employees)
                        .ThenInclude(n => n.Employee)
                .Include(x => x.Workflow)
                .Include(x => x.Status)
                .Include(x => x.CreationUser)
                .Include(x => x.UserApplicant)
                .Include(x => x.Histories)
                .FirstOrDefault();
        }

        public Domain.Models.RequestNote.RequestNote GetProviders(int requestNoteID)
        {
            return this.context.RequestNote.Include(x => x.Providers)
                    .ThenInclude(p => p.Provider)
                    .FirstOrDefault(x => x.Id == requestNoteID);
                  
        }


        public IList<Domain.Models.RequestNote.RequestNote> GetAll(RequestNoteGridFilters filters)
        {
            if (filters == null)
                filters = new RequestNoteGridFilters();
            string emailUsuario = "";
            if (filters.CreationUserId.HasValue)
                emailUsuario = context.Employees.FirstOrDefault(u => u.Id == filters.CreationUserId)?.Email;
            if (filters.FromDate.HasValue)
                filters.FromDate = filters.FromDate.Value.Date;
            if (filters.ToDate.HasValue)
                filters.ToDate = filters.ToDate.Value.Date.AddDays(1);


            return this.context.RequestNote
                .Where(n => !filters.Id.HasValue || n.Id == filters.Id.Value)
                .Where(n => !filters.CreationUserId.HasValue || n.CreationUser.Email == emailUsuario)
                .Where(n => !filters.StatusId.HasValue || n.StatusId == filters.StatusId)
                .Where(n => !filters.FromDate.HasValue || n.CreationDate >= filters.FromDate)
                .Where(n => !filters.ToDate.HasValue || n.CreationDate < filters.ToDate)
                .Where(n => !filters.AnalyticID.HasValue || n.Analytics.Any(p => p.AnalyticId == filters.AnalyticID))
                .Where(n => !filters.ProviderId.HasValue || n.Providers.Any(p=> p.ProviderId == filters.ProviderId))
                //Provider? Quieren filtrar por todos o selected?
                .Include(x => x.Status)
                .Include(x => x.Analytics)
                    .ThenInclude(p => p.Analytic)
                .Include(x => x.CreationUser)
                .OrderByDescending(x=>x.Id)
                .ToList();
        }

        public void UpdateRequestNote(Domain.Models.RequestNote.RequestNote requestNote)
        {
            this.context.RequestNote.Update(requestNote);
        }

        public void InsertRequestNote(Domain.Models.RequestNote.RequestNote requestNote)
        {
            this.context.RequestNote.Add(requestNote);
        }

        public void Save()
        {
            context.SaveChanges();
        }


        public bool IsCompletelyDelivered(int requestNoteId)
        {
            var requested = context.RequestNoteProductService.Where(p => p.RequestNoteId == requestNoteId)
                .Select(p=> new { Id = p.Id, Quantity = p.Quantity });
            var delivered = context.BuyOrderProductService.Where(p => p.BuyOrder.RequestNoteId == requestNoteId)
                .Select(p => new { Id = p.RequestNoteProductServiceId, Quantity = p.DeliveredQuantity ?? 0 });
            var pending = requested.Any(r=> r.Quantity < delivered.Where(d=> d.Id == r.Id).Sum(d=> d.Quantity));
            return pending;
        }
    }   
}
