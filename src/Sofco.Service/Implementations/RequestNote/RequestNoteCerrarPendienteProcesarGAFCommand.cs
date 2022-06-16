using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteCerrarPendienteProcesarGAFCommand : IRequestNoteCommand<RequestNoteSubmitPendienteProcesarGAFDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteCerrarPendienteProcesarGAFCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitPendienteProcesarGAFDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitPendienteProcesarGAFDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitPendienteProcesarGAFDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitPendienteProcesarGAFDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
