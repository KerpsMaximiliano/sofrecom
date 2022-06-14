namespace Sofco.Core.DAL.RequestNote
{
    public interface IRequestNoteAnalitycRepository
    {
        Domain.Models.RequestNote.RequestNoteAnalytic GetById(int id);
        void UpdateAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc);
        void InsertAnalityc(Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc);
        void Save();
    }
}
