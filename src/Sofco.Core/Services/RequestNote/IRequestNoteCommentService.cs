using Sofco.Core.Models.RequestNote;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.RequestNote
{
    public interface IRequestNoteCommentService
    {
        IList<Comments> GetByRequestNoteId(int id);
        Response<int> Add(Comments requestNoteComment);
        Response<int> Delete(int id);
    }
}
