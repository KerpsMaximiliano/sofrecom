using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteBorrador")]
    [ApiController]
    public class RequestNoteBorradorController : Controller
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteBorradorController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("GuardarBorrador")]
        public IActionResult GuardarBorrador([FromBody] RequestNoteModel requestNote)
        {
            var response = this._requestNoteService.GuardarBorrador(requestNote);

            return this.CreateResponse(response);
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles()
        {
            var response = new Response<List<File>>();

            if (Request.Form.Files.Any())
            {
                await _requestNoteService.AttachFiles(response, Request.Form.Files.ToList());
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }


        [HttpPost("AprobarBorrador")]
        public IActionResult AprobarBorrador(int id)
        {
            this._requestNoteService.ChangeStatus(new RequestNoteModel() { Id = id }, Domain.RequestNoteStates.RequestNoteStates.PendienteRevisiónAbastecimiento);

            return Ok();
        }

        [HttpPost("RechazarBorrador")]
        public IActionResult RechazarBorrador(int id)
        {
            this._requestNoteService.ChangeStatus(new RequestNoteModel() { Id = id }, Domain.RequestNoteStates.RequestNoteStates.Rechazada);

            return Ok();
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            return Ok(this._requestNoteService.GetById(id));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(int id)
        {
            return Ok(this._requestNoteService.GetAll());
        }
    }
}