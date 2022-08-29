using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteProviderService
    {
        Domain.Models.RequestNote.RequestNoteProvider GetById(int id);
        IList<Domain.Models.RequestNote.RequestNoteProvider> GetAll();
        IList<Domain.Models.RequestNote.RequestNoteProvider> GetFilesByProviderId(int providerId);
        void Save(Domain.Models.RequestNote.RequestNoteProvider requestNoteProvider);
        void GuardarArchivo(Domain.Models.RequestNote.RequestNoteProvider requestNoteProvider);
        void GuardarArchivosProvider(List<Domain.Models.RequestNote.RequestNoteProvider> requestNoteProviders);
    }
}
