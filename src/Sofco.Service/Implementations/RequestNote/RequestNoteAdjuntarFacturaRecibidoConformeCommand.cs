using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteAdjuntarFacturaRecibidoConformeCommand : IRequestNoteCommand<RequestNoteSubmitRecibidoConformeDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteAdjuntarFacturaRecibidoConformeCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitRecibidoConformeDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitRecibidoConformeDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitRecibidoConformeDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitRecibidoConformeDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
