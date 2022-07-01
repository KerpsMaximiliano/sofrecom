using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.RequestNote
{
    public interface IRequestNoteProviderRepository
    {
        Domain.Models.RequestNote.RequestNoteProvider GetById(int id);
        List<Domain.Models.RequestNote.RequestNoteProvider> GetAll();
        List<Domain.Models.RequestNote.RequestNoteProvider> GetFilesByProviderId(int providerId);
        void InsertarRequestNoteProvider(Domain.Models.RequestNote.RequestNoteProvider requestNoteProvider);
        void Save();

        void Delete(RequestNoteProvider provider);
    }
}
