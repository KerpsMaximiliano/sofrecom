using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Core.DAL.RequestNote;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.DAL.Repositories.Common;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteRepository : BaseRepository<Sofco.Domain.Models.RequestNote.RequestNote>, IRequestNoteRepository
    {
        public RequestNoteRepository(SofcoContext context) : base(context)
        {
        }

    }   
}
