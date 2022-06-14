using Sofco.Core.DAL.RequestNote;
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
    }
}
