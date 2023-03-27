using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteComment")]
    [ApiController]
    public class RequestNoteCommentController : Controller
    {
        private readonly IRequestNoteCommentService requestNoteCommentService;

        public RequestNoteCommentController(IRequestNoteCommentService requestNoteCommentService)
        {
            this.requestNoteCommentService = requestNoteCommentService;
        }

        [HttpGet("GetByNoteRequest")]
        public IActionResult GetByNoteRequest(int id)
        {
            return Ok(requestNoteCommentService.GetByRequestNoteId(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] Comments model)
        {
            var response = requestNoteCommentService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var response = requestNoteCommentService.Delete(id);

            return this.CreateResponse(response);
        }
    }
}