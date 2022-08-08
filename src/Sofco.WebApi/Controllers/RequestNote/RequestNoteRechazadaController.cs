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
    public class RequestNoteRechazadaController : RequestNoteAbstractWorkflowController<RequestNoteSubmitRechazadaDTO, RequestNoteLoadRechazadaDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteRechazadaController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadRechazadaDTO> Get(int id)
        {
            return new Response<RequestNoteLoadRechazadaDTO>();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitRechazadaDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitRechazadaDTO>> map =
                 new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitRechazadaDTO>>();
            return map;
        }
    }
}