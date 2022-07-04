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
            return this.context.RequestNoteAnalytic.Where(x => x.AnalyticId == id).ToList().FirstOrDefault();
        }

        public void UpdateAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc)
        {
            this.context.RequestNoteAnalytic.Update(requestNoteAnalityc);
        }

        public void InsertAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc)
        {
            this.context.RequestNoteAnalytic.Add(requestNoteAnalityc);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public List<RequestNoteAnalytic> GetByRequestNoteId(int requestNoteId)
        {
            return this.context.RequestNoteAnalytic.Where(x => x.RequestNoteId == requestNoteId).ToList();
        }

        public void Delete(RequestNoteAnalytic analytic)
        {
            this.context.RequestNoteAnalytic.Remove(analytic);
        }
    }
}
