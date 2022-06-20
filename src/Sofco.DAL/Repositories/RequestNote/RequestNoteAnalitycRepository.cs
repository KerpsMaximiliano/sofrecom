using Sofco.Core.DAL.RequestNote;
using Sofco.Domain.Models.RequestNote;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteAnalitycRepository : IRequestNoteAnalitycRepository
    {
        protected readonly SofcoContext context;

        public RequestNoteAnalitycRepository(SofcoContext context)
        {
            this.context = context;
        }

        public Domain.Models.RequestNote.RequestNoteAnalytic GetById(int id)
        {
            return this.context.RequestNoteAnalytics.Where(x => x.AnalyticId == id).ToList().FirstOrDefault();
        }

        public void UpdateAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc)
        {
            this.context.RequestNoteAnalytics.Update(requestNoteAnalityc);
        }

        public void InsertAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc)
        {
            this.context.RequestNoteAnalytics.Add(requestNoteAnalityc);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public List<RequestNoteAnalytic> GetByRequestNoteId(int requestNoteId)
        {
            return this.context.RequestNoteAnalytics.Where(x => x.RequestNoteId == requestNoteId).ToList();
        }
    }
}
