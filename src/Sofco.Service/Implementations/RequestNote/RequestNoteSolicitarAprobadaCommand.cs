using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteSolicitarAprobadaCommand : IRequestNoteCommand<RequestNoteSubmitAprobadaDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteSolicitarAprobadaCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitAprobadaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitAprobadaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitAprobadaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitAprobadaDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
