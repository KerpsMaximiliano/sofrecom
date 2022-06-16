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
    public class RequestNoteCerradaController : RequestNoteAbstractWorkflowController<RequestNoteSubmitCerradaDTO, RequestNoteLoadCerradaDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteCerradaController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadCerradaDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitCerradaDTO>> GetActionDictionary()
        {
            throw new NotImplementedException();
        }
    }
}