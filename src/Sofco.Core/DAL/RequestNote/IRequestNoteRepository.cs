﻿using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.RequestNote;

namespace Sofco.Core.DAL.RequestNote
{
    public interface IRequestNoteRepository : IBaseRepository<Sofco.Domain.Models.RequestNote.RequestNote>
    {
        Domain.Models.RequestNote.RequestNote GetById(int id);
        IList<Domain.Models.RequestNote.RequestNote> GetAll(RequestNoteGridFilters filters);
        void UpdateRequestNote(Domain.Models.RequestNote.RequestNote requestNote);
        void InsertRequestNote(Domain.Models.RequestNote.RequestNote requestNote);
        void Save();
    }
}
