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
    public class RequestNoteSolicitadaProveedorController : RequestNoteAbstractWorkflowController<RequestNoteSubmitSolicitadaProveedorDTO, RequestNoteLoadSolicitadaProveedorDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteSolicitadaProveedorController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadSolicitadaProveedorDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitSolicitadaProveedorDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitSolicitadaProveedorDTO>> map =
               new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitSolicitadaProveedorDTO>>
               {
                    { "Confirmar", new RequestNoteConfirmarRecepcionSolicitadaProveedorCommand(_requestNoteService) }
               };
            return map;
        }
    }
}