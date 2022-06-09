using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteCerrarSolicitadaProveedorCommand : IRequestNoteCommand<RequestNoteSubmitSolicitadaProveedorDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteCerrarSolicitadaProveedorCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitSolicitadaProveedorDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitSolicitadaProveedorDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitSolicitadaProveedorDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitSolicitadaProveedorDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
