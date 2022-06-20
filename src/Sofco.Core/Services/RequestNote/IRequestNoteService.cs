using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.RequestNoteStates;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteService
    {
        Response SaveBorrador(RequestNoteSubmitDTO dto);
        Response SavePendienteRevisionAbastecimiento(RequestNoteSubmitDTO dto);
        //
        void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId);
        void RechazarRequestNote(int requestNodeId);
        void GuardarBorrador(Domain.Models.RequestNote.RequestNote requestNoteBorrador);
        Domain.Models.RequestNote.RequestNote GetById(int id);
        IList<Domain.Models.RequestNote.RequestNote> GetAll();
        void ChangeStatus(int requestNodeId, RequestNoteStates requestNoteStates);
    }
}
