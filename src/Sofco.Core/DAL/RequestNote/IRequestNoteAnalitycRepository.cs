using System.Collections.Generic;

namespace Sofco.Core.DAL.RequestNote
{
    public interface IRequestNoteAnalitycRepository
    {
        Domain.Models.RequestNote.RequestNoteAnalytic GetById(int id);
        List<Domain.Models.RequestNote.RequestNoteAnalytic> GetByRequestNoteId(int requestNoteId);
        void UpdateAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc);
        void InsertAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc);

        void Delete(Domain.Models.RequestNote.RequestNoteAnalytic analytic);
        void Save();
    }
}
