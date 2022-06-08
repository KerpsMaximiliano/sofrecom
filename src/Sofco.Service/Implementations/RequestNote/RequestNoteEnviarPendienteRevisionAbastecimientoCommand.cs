using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteEnviarPendienteRevisionAbastecimientoCommand : 
        IRequestNoteCommand<RequestNoteSubmitNuevoDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteEnviarPendienteRevisionAbastecimientoCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitNuevoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitNuevoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitNuevoDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitNuevoDTO dto)
        {
            throw new NotImplementedException();
        }
    }

    
}
