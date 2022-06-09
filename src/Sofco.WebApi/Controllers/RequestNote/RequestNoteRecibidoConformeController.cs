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
    public class RequestNoteRecibidoConformeController : RequestNoteAbstractWorkflowController<RequestNoteSubmitRecibidoConformeDTO, RequestNoteLoadRecibidoConformeDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteRecibidoConformeController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadRecibidoConformeDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitRecibidoConformeDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitRecibidoConformeDTO>> map =
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitRecibidoConformeDTO>>
                {
                    { "Adjuntar", new RequestNoteAdjuntarFacturaRecibidoConformeCommand(_requestNoteService) }
                };
            return map;
        }
    }
}