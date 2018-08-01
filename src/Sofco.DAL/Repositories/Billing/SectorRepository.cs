using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.Billing
{
    public class SectorRepository : BaseRepository<Sector>, ISectorRepository
    {
        public SectorRepository(SofcoContext context) : base(context)
        {
        }

        public new List<Sector> GetAll()
        {
            return context.Sectors
                .Include(s => s.ResponsableUser)
                .ToList();
        }
    }
}
