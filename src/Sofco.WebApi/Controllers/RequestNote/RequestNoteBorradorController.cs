using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    public class RequestNoteBorradorController : RequestNoteAbstractWorkflowController<RequestNoteSubmitBorradorDTO, RequestNoteLoadBorradorDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteBorradorController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadBorradorDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitBorradorDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitBorradorDTO>> map =
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitBorradorDTO>>
                {
                    { "Grabar", new RequestNoteGrabarBorradorCommand(_requestNoteService) },
                    { "Enviar", new RequestNoteEnviarBorradorCommand(_requestNoteService) }
                };
            return map;
        }
    }
}