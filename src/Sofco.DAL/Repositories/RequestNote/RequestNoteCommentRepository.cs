using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.RequestNote;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteCommentRepository : IRequestNoteCommentRepository
    {
        protected readonly SofcoContext context;

        public RequestNoteCommentRepository(SofcoContext context)
        {
            this.context = context;
        }

        public RequestNoteComment GetById(int id)       
        {
            return this.context.RequestNoteComment.Where(x => x.Id == id).ToList().FirstOrDefault();
        }

        public void Add(RequestNoteComment requestNoteComment)
        {
            context.RequestNoteComment.Add(requestNoteComment);

        }

        public List<RequestNoteComment> GetByRequestNoteId(int id)
        {
            return this.context.RequestNoteComment.Where(x => x.RequestNoteId == id)
                .ToList();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Delete(RequestNoteComment entity)
        {
            this.context.RequestNoteComment.Remove(entity);
        }
    }
}
