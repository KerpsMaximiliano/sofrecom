using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Models.RequestNote;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.RequestNoteStates;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteService : IRequestNoteService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteService> logger;

        public RequestNoteService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response SaveBorrador(RequestNoteSubmitDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response SavePendienteRevisionAbastecimiento(RequestNoteSubmitDTO dto)
        {
            throw new NotImplementedException();
        }


        public void RechazarRequestNote(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);

            requestNote.StatusId = (int)RequestNoteStates.Reachazada;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);
            requestNote.StatusId = (int)RequestNoteStates.PendienteAprobaciónGerentesAnalítica;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public void GuardarBorrador(Domain.Models.RequestNote.RequestNote requestNoteBorrador)
        {
            requestNoteBorrador.StatusId = (int)RequestNoteStates.Borrador;

            foreach(RequestNoteAnalytic requestNoteAnalytic in requestNoteBorrador.Analytics)
            {
                requestNoteAnalytic.Id = 0;
            }

            this.unitOfWork.RequestNoteRepository.InsertRequestNote(requestNoteBorrador);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public Domain.Models.RequestNote.RequestNote GetById(int id)
        {
            return this.unitOfWork.RequestNoteRepository.GetById(id);
        }

        public IList<Domain.Models.RequestNote.RequestNote> GetAll()
        {
            return this.unitOfWork.RequestNoteRepository.GetAll();
        }

        public void ChangeStatus(int requestNodeId, RequestNoteStates requestNoteStates)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);

            requestNote.StatusId = (int)requestNoteStates;

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }
    }
}
