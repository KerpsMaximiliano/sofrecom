﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.RequestNote;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
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
                .Include(x=> x.ProviderArea)
                .Include(x => x.ProductsServices)
                .Include(x => x.Providers)
                    .ThenInclude(p => p.File)
                .Include(x => x.Providers)
                    .ThenInclude(p => p.Provider)
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
                .FirstOrDefault();
        }

        public IList<Domain.Models.RequestNote.RequestNote> GetAll()
        {
            return this.context.RequestNote.ToList();
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
    }   
}
