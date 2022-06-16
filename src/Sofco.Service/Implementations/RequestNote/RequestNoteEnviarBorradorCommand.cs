using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteEnviarBorradorCommand : IRequestNoteCommand<RequestNoteSubmitBorradorDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteEnviarBorradorCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitBorradorDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitBorradorDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitBorradorDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitBorradorDTO dto)
        {
            throw new NotImplementedException();
        }
    }

    
}
