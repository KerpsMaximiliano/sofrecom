using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteEnviarPendienteRevisionAbastecimientoCommand : IRequestNoteCommand<RequestNoteSubmitPendienteRevisionAbastecimientoDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteEnviarPendienteRevisionAbastecimientoCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitPendienteRevisionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitPendienteRevisionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitPendienteRevisionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitPendienteRevisionAbastecimientoDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
