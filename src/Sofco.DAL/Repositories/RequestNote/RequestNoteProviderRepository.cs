using Sofco.Core.DAL.RequestNote;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteProviderRepository : IRequestNoteProviderRepository
    {
        protected readonly SofcoContext context;

        public RequestNoteProviderRepository(SofcoContext context)
        {
            this.context = context;
        }

        public RequestNoteProvider GetById(int id)
        {
            return this.context.RequestNoteProvider.Where(x => x.Id == id).ToList().FirstOrDefault();
        }

        public List<RequestNoteProvider> GetAll()
        {
            return this.context.RequestNoteProvider.ToList();
        }

        public List<RequestNoteProvider> GetFilesByProviderId(int providerId)
        {
            return this.context.RequestNoteProvider.Where(x => x.ProviderId == providerId).ToList();
        }

        public void InsertarRequestNoteProvider(RequestNoteProvider requestNoteProvider)
        {
            context.RequestNoteProvider.Add(requestNoteProvider);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
