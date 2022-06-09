using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteAprobarPendienteAprobacionDAFCommand : IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionDAFDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteAprobarPendienteAprobacionDAFCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitPendienteAprobacionDAFDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitPendienteAprobacionDAFDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitPendienteAprobacionDAFDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitPendienteAprobacionDAFDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
