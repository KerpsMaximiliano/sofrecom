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
    public class RequestNotePendienteProcesarGAFController : RequestNoteAbstractWorkflowController<RequestNoteSubmitPendienteProcesarGAFDTO, RequestNoteLoadPendienteProcesarGAFDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteProcesarGAFController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadPendienteProcesarGAFDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteProcesarGAFDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteProcesarGAFDTO>> map =
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteProcesarGAFDTO>>
                {
                    { "Cerrar", new RequestNoteCerrarPendienteProcesarGAFCommand(_requestNoteService) }
                };
            return map;
        }
    }
}