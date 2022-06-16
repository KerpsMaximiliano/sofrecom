using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteRechazarPendienteAprobacionAbastecimientoCommand : IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionAbastecimientoDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteRechazarPendienteAprobacionAbastecimientoCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitPendienteAprobacionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitPendienteAprobacionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitPendienteAprobacionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitPendienteAprobacionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
