using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteAprobarFacturaPendienteAprobacionGerenteCommand : IRequestNoteCommand<RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteAprobarFacturaPendienteAprobacionGerenteCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
