using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Rrhh;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.DAL.Repositories.Rrhh
{
    public class CloseDateRepository : BaseRepository<CloseDate>, ICloseDateRepository
    {
        public CloseDateRepository(SofcoContext context) : base(context)
        {
        }

        public IList<CloseDate> Get(int startMonth, int startYear, int endMonth, int endYear)
        {
            return context.CloseDates.Where(x =>
                    new DateTime(x.Year, x.Month, 1).Date >= new DateTime(startYear, startMonth, 1).Date &&
                    new DateTime(x.Year, x.Month, 1).Date <= new DateTime(endYear, endMonth, 1).Date)
                .ToList();
        }
    }
}
