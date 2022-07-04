using Sofco.Core.DAL.RequestNote;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.RequestNote;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.RequestNote
{
    public class RequestNoteProductServiceRepository : BaseRepository<RequestNoteFile>
    {
        
        public RequestNoteProductServiceRepository(SofcoContext context) : base(context)
        {
        }

    }
}
