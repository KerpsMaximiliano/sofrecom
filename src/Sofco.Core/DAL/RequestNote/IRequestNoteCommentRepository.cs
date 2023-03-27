using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.RequestNote
{
    public interface IRequestNoteCommentRepository
    {

        RequestNoteComment GetById(int id);
        void Delete(RequestNoteComment entity);
        List<RequestNoteComment> GetByRequestNoteId(int id);
        void Add(RequestNoteComment requestNoteComment);
        void Save();
    }
}
