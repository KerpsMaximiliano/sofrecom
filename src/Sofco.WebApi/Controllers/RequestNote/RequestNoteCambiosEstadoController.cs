﻿using System;
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
    [Route("api/RequestNoteCambiosEstado")]
    [ApiController]
    public class RequestNoteCambiosEstadoController : ControllerBase
    {
        private readonly IRequestNoteService requestNoteService;

        public RequestNoteCambiosEstadoController(IRequestNoteService requestNoteService)
        {
            this.requestNoteService = requestNoteService;
        }
        /*
        #region Pendiente Procesar GAF

        [HttpPost("AprobarPendienteProcesarGAF")]
        public IActionResult AprobarPendienteProcesarGAF([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.Cerrada);

            return Ok();
        }
        #endregion

        #region Aprobada
        [HttpPost("AprobarAprobada")]
        public IActionResult AprobarRequestNote([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.SolicitadaAProveedor);

            return Ok();
        }

        [HttpPost("RechazarAprobada")]
        public IActionResult RechazarRequestNode([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.Rechazada);

            return Ok();
        }
        #endregion

        #region Factura Pendiente Aprobación Gerente
        [HttpPost("AprobarFacturaPendienteAprobacion")]
        public IActionResult AprobarFacturaPendienteAprobacion([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.PendienteProcesarGAF);

            return Ok();
        }
        #endregion

        #region Pendiente Aprobación Abastecimiento
        [HttpPost("AprobarPendienteAprobacionAbastecimiento")]
        public IActionResult AprobarPendienteAprobacionAbastecimiento([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.PendienteAprobaciónDAF);

            return Ok();
        }

        [HttpPost("RechazarPendienteAprobacionAbastecimiento")]
        public IActionResult RechazarPendienteAprobacionAbastecimiento([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.Rechazada);

            return Ok();
        }
        #endregion

        #region Pendiente Aprobación DAF
        [HttpPost("AprobarPendienteAprobacionDAF")]
        public IActionResult AprobarPendienteAprobacionDAF([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.Aprobada);

            return Ok();
        }

        [HttpPost("RechazarPendienteAprobacionDAF")]
        public IActionResult RechazarPendienteAprobacionDAF([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.PendienteAprobaciónAbastecimiento);

            return Ok();
        }
        #endregion

        #region Pendiente Aprobación Gerentes Analítica
        [HttpPost("AprobarPendienteAprobacionGerentesAnalitica")]
        public IActionResult AprobarPendienteAprobacionGerentesAnalitica([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.PendienteAprobaciónAbastecimiento);

            return Ok();
        }

        [HttpPost("RechazarPendienteAprobacionGerentesAnalitica")]
        public IActionResult RechazarPendienteAprobacionGerentesAnalitica([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.PendienteRevisiónAbastecimiento);

            return Ok();
        }
        #endregion

        #region Pendiente Revisión Abastecimiento
        [HttpPost("AprobarPendienteRevisionAbastecimiento")]
        public IActionResult AprobarPendienteRevisionAbastecimiento([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.PendienteAprobaciónGerentesAnalítica);

            return Ok();
        }

        [HttpPost("RechazarPendienteRevisionAbastecimiento")]
        public IActionResult RechazarPendienteRevisionAbastecimiento([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.Rechazada);

            return Ok();
        }
        #endregion

        #region Recibido Conforme
        [HttpPost("AprobarRecibidoConforme")]
        public IActionResult AprobarRecibidoConforme([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.FacturaPendienteAprobaciónGerente);

            return Ok();
        }
        #endregion

        #region Solicitada a Proveedor
        [HttpPost("CerrarSolicitadaProveedor")]
        public IActionResult CerradaSolicitadaProveedor([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.Cerrada);
            return Ok();
        }


        [HttpPost("AprobarSolicitadaProveedor")]
        public IActionResult AprobarSolicitadaProveedor([FromBody] RequestNoteModel requestNote)
        {
            this.requestNoteService.SaveChanges(requestNote, Domain.RequestNoteStates.RequestNoteStatus.RecibidoConforme);
            return Ok();
        }
        #endregion
        */
    }
}