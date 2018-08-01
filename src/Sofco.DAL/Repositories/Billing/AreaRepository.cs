using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.Billing
{
    public class AreaRepository : BaseRepository<Area>, IAreaRepository
    {
        public AreaRepository(SofcoContext context) : base(context)
        {
        }

        public new List<Area> GetAll()
        {
            return context.Areas
                .Include(s => s.ResponsableUser)
                .ToList();
        }

        public Area GetWithResponsable(int id)
        {
            return context.Areas.Include(x => x.ResponsableUser).SingleOrDefault(x => x.Id == id);
        }
    }
}
