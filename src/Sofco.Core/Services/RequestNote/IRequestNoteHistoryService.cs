using Sofco.Core.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteHistoryService
    {
        IList<History> GetByRequestNoteId(int id);
    }
}
