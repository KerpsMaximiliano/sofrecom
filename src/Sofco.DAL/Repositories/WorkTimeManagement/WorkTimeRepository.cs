using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.DAL.Repositories.WorkTimeManagement
{
    public class WorkTimeRepository : BaseRepository<WorkTime>, IWorkTimeRepository
    {
        public WorkTimeRepository(SofcoContext context) : base(context)
        {
        }

        public IList<WorkTime> Get(DateTime startDate, DateTime endDate, int currentUserId)
        {
            return context.WorkTimes
                .Where(x => x.UserId == currentUserId && x.Date >= startDate && x.Date <= endDate)
                .Include(x => x.Employee)
                .Include(x => x.Analytic)
                .Include(x => x.Task)
                .ToList();
        }
    }
}
