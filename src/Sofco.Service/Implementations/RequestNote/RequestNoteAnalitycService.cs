using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteAnalitycService : IRequestNoteAnalitycService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RequestNoteAnalitycService> logger;

        public RequestNoteAnalitycService(IUnitOfWork unitOfWork, ILogMailer<RequestNoteAnalitycService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public void CambiarAPendienteAprobacion(int id)
        {
            Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc = this.unitOfWork.RequestNoteAnalitycRepository.GetById(id);

            requestNoteAnalityc.Status = "Pendiente Aprobación";

            this.unitOfWork.RequestNoteAnalitycRepository.UpdateAnalityc(requestNoteAnalityc);
            this.unitOfWork.RequestNoteAnalitycRepository.Save();
        }

        public void Rechazar(int id)
        {
            Domain.Models.RequestNote.RequestNoteAnalytic requestNoteAnalityc = this.unitOfWork.RequestNoteAnalitycRepository.GetById(id);

            requestNoteAnalityc.Status = "Rechazado";

            this.unitOfWork.RequestNoteAnalitycRepository.UpdateAnalityc(requestNoteAnalityc);
            this.unitOfWork.RequestNoteAnalitycRepository.Save();
        }
    }
}
