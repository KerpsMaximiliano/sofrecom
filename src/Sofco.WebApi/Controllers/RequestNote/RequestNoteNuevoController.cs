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
    public class RequestNoteNuevoController : RequestNoteAbstractWorkflowController<RequestNoteSubmitNuevoDTO, RequestNoteLoadNuevoDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteNuevoController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadNuevoDTO> Get(int id)
        {
            // Devolver solo los campos que corresponden
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitNuevoDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitNuevoDTO>> map = new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitNuevoDTO>>();
            map.Add("Guardar", new RequestNoteNuevoCommand(_requestNoteService));
            map.Add("Enviar", new RequestNoteEnviarPendienteRevisionAbastecimientoCommand(_requestNoteService));
            return map;
        }

    }
}