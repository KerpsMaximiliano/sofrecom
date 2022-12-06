using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.RequestNoteStates;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteService
    {
        Task<Response<List<File>>> AttachFiles(Response<List<File>> response, List<IFormFile> files);
        void CambiarAPendienteApobacionGerenteAnalitica(int requestNodeId);
        void RechazarRequestNote(int requestNodeId);
        Response<int> GuardarBorrador(RequestNoteModel requestNoteBorrador);
        Response<RequestNoteModel> GetById(int id);
        IList<RequestNoteGridModel> GetAll(RequestNoteGridFilters filters);
        void ChangeStatus(RequestNoteModel requestNote, RequestNoteStatus requestNoteStates);
    }
}
