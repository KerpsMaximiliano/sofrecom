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
    public class RequestNoteFacturaPendienteAprobacionGerenteController : RequestNoteAbstractWorkflowController<RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO, RequestNoteLoadFacturaPendienteAprobacionGerenteDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteFacturaPendienteAprobacionGerenteController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadFacturaPendienteAprobacionGerenteDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO>> map =
                 new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitFacturaPendienteAprobacionGerenteDTO>>
                 {
                    { "Grabar", new RequestNoteAprobarFacturaPendienteAprobacionGerenteCommand(_requestNoteService) }
                 };
            return map;
        }
    }
}