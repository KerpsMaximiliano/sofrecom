using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.RequestNote
{
    public interface IRequestNoteHistoryRepository
    {
        RequestNoteHistory GetById(int id);
        List<RequestNoteHistory> GetAll();
        List<RequestNoteHistory> GetByRequestNoteId(int id);
        void Save();

    }
}
