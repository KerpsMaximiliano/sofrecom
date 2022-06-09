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
    public class RequestNoteAprobadaController : RequestNoteAbstractWorkflowController<RequestNoteSubmitAprobadaDTO, RequestNoteLoadAprobadaDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteAprobadaController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadAprobadaDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitAprobadaDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitAprobadaDTO>> map = 
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitAprobadaDTO>>();
            map.Add("Solicitar", new RequestNoteSolicitarAprobadaCommand(_requestNoteService));
            return map;
    }
    }
}