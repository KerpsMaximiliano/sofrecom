using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteHistories")]
    [ApiController]
    public class RequestNoteHistoriesController : Controller
    {
        private readonly IRequestNoteHistoryService _requestNoteHistoryService;

        public RequestNoteHistoriesController(IRequestNoteHistoryService requestNoteHistoryService)
        {
            this._requestNoteHistoryService = requestNoteHistoryService;
        }

        public IActionResult GetByNoteRequest(int id)
        {
            return Ok(_requestNoteHistoryService.GetByRequestNoteId(id));
        }

    }
}