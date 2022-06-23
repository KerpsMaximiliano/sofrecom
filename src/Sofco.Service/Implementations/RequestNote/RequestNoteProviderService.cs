using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteProviderService : IRequestNoteProviderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteService> logger;

        public RequestNoteProviderService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public IList<Domain.Models.RequestNote.RequestNoteProvider> GetAll()
        {
            return this.unitOfWork.RequestNoteProviderRepository.GetAll();
        }

        public IList<Domain.Models.RequestNote.RequestNoteProvider> GetFilesByProviderId(int providerId)
        {
            return this.unitOfWork.RequestNoteProviderRepository.GetAll();
        }

        public Domain.Models.RequestNote.RequestNoteProvider GetById(int id)
        {
            return this.unitOfWork.RequestNoteProviderRepository.GetById(id);
        }

        public void Save(Domain.Models.RequestNote.RequestNoteProvider requestNoteProvider)
        {
            this.unitOfWork.RequestNoteProviderRepository.InsertarRequestNoteProvider(requestNoteProvider);
            this.unitOfWork.RequestNoteProviderRepository.Save();
        }

        public void GuardarArchivo(Domain.Models.RequestNote.RequestNoteProvider requestNoteProvider)
        {
            //modificamos el PurchaseOrderNumber 
            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNoteProvider.RequestNote);
            this.unitOfWork.RequestNoteRepository.Save();

            //guardamos el nuevo archivo para el provider
            this.unitOfWork.RequestNoteProviderRepository.InsertarRequestNoteProvider(requestNoteProvider);
            this.unitOfWork.RequestNoteProviderRepository.Save();
        }

        public void GuardarArchivosProvider(List<RequestNoteProvider> requestNoteProviders)
        {
            foreach(RequestNoteProvider requestNoteProvider in requestNoteProviders)
            {
                Save(requestNoteProvider);
            }
        }
    }
}
