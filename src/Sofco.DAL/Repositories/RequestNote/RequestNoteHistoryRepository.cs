using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.RequestNote;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteHistoryRepository : IRequestNoteHistoryRepository
    {
        protected readonly SofcoContext context;

        public RequestNoteHistoryRepository(SofcoContext context)
        {
            this.context = context;
        }

        public RequestNoteHistory GetById(int id)
        {
            return this.context.RequestNoteHistories.Where(x => x.Id == id).ToList().FirstOrDefault();
        }
        
        public List<RequestNoteHistory> GetAll()
        {
            return this.context.RequestNoteHistories.ToList();
        }

        public List<RequestNoteHistory> GetByRequestNoteId(int id)
        {
            return this.context.RequestNoteHistories.Where(x => x.RequestNoteId == id)
                .Include(x=> x.StatusFrom)
                .Include(x => x.StatusTo)
                .ToList();
        }


        public void Save()
        {
            context.SaveChanges();
        }
    }
}
