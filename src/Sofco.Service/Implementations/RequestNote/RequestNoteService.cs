using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Models.Workflow;
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

        public Response Get(int id)
        {
            throw new NotImplementedException();
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

            requestNote.StatusId = 8; //Pendiente Aprobación Gerente Analíticas MATOO

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId)
        {
            Domain.Models.RequestNote.RequestNote requestNote = this.unitOfWork.RequestNoteRepository.GetById(requestNodeId);
            requestNote.StatusId = 9; //Pendiente Aprobación Gerente Analíticas MATOO

            this.unitOfWork.RequestNoteRepository.UpdateRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }

        public void GuardarBorrador(RequestNoteSubmitBorradorDTO requestNoteBorrador)
        {
            Domain.Models.RequestNote.RequestNote requestNote = new Domain.Models.RequestNote.RequestNote();
            //mapeamos el objeto que llega al que se usa
            requestNote.Comments = requestNoteBorrador.Comments;
            requestNote.ConsideredInBudget = requestNoteBorrador.ConsideredInBudget;
            requestNote.Description = requestNoteBorrador.Description;
            requestNote.EvalpropNumber = requestNoteBorrador.EvalpropNumber;
            requestNote.ProviderAreaId = requestNoteBorrador.ProviderAreaId;
            requestNote.RequiresEmployeeClient = requestNoteBorrador.RequiresEmployeeClient;


            this.unitOfWork.RequestNoteRepository.InsertRequestNote(requestNote);
            this.unitOfWork.RequestNoteRepository.Save();
        }
    }
}
